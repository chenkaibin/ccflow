using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Port;

public partial class SDKFlowDemo_DemoEntity : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
    /// <summary>
    /// 日志应用
    /// </summary>
    public void LogApp()
    {
        // 写入一条消息.
        BP.DA.Log.DefaultLogWriteLineInfo("这是一条消息. ");

        // 写入一条警告.
        BP.DA.Log.DefaultLogWriteLineWarning("这是一条警告. ");

        // 写入一条异常或者错误.
        BP.DA.Log.DefaultLogWriteLineError("这是一条错误. ");
    }
    /// <summary>
    /// 全局的基本应用,获取当前操作员的信息.
    /// </summary>
    public void GloBaseApp()
    {
        // 执行登陆。
        Emp emp = new Emp("guobaogeng");
        BP.Web.WebUser.SignInOfGener(emp);

        // 当前登陆人员编号.
        string currLoginUserNo = BP.Web.WebUser.No;
        // 登陆人员名称
        string currLoginUserName = BP.Web.WebUser.Name;
        // 登陆人员部门编号.
        string currLoginUserDeptNo = BP.Web.WebUser.FK_Dept;
        // 登陆人员部门名称
        string currLoginUserDeptName = BP.Web.WebUser.FK_DeptName;

        BP.Web.WebUser.Exit(); //执行退出.
    }
    /// <summary>
    /// 数据库操作访问
    /// </summary>
    public void DataBaseAccess()
    {
        #region 执行不带有参数.
        // 执行Insert ,delete, update 语句.
       int result= BP.DA.DBAccess.RunSQL("DELETE FROM Port_Emp WHERE 1=2");

        // 执行多个sql
        string sqls = "DELETE FROM Port_Emp WHERE 1=2";
        sqls += "@DELETE FROM Port_Emp WHERE 1=2";
        sqls += "@DELETE FROM Port_Emp WHERE 1=2";
        BP.DA.DBAccess.RunSQLs(sqls);

        //执行查询返回datatable.
        string sql = "SELECT * FROM Port_Emp";
        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

        //执行查询返回 string 值.
        sql = "SELECT FK_Dept FROM Port_Emp WHERE No='" + BP.Web.WebUser.No + "'";
        string fk_dept = BP.DA.DBAccess.RunSQLReturnString(sql);

        //执行查询返回 int 值. 也可以返回float, string
        sql = "SELECT count(*) as Num FROM Port_Emp ";
        int empNum = BP.DA.DBAccess.RunSQLReturnValInt(sql);

        //运行存储过程.
        string spName = "MySp";
        BP.DA.DBAccess.RunSP(spName);
        #endregion 执行不带有参数.

        #region 执行带有参数.
        // 执行Insert ,delete, update 语句.
        // 已经明确数据库类型.
        Paras ps = new Paras();
        ps.SQL = "DELETE FROM Port_Emp WHERE No=@UserNo";
        ps.Add("UserNo", "abc");
        BP.DA.DBAccess.RunSQL(ps);

        // 不知道数据库类型.
        ps = new Paras();
        ps.SQL = "DELETE FROM Port_Emp WHERE No=" + BP.SystemConfig.AppCenterDBVarStr + "UserNo";
        ps.Add("UserNo", "abc");
        BP.DA.DBAccess.RunSQL(ps);


        //执行查询返回datatable.
        ps = new Paras();
        ps.SQL = "SELECT * FROM Port_Emp WHERE FK_Dept=@DeptNoVar";
        ps.Add("DeptNoVar", "0102");
        DataTable dtDept = BP.DA.DBAccess.RunSQLReturnTable(ps);

        //运行存储过程.
        ps = new Paras();
        ps.Add("DeptNoVar", "0102");
        spName = "MySp";
        BP.DA.DBAccess.RunSP(spName, ps);
        #endregion 执行带有参数.
    }
    /// <summary>
    /// Entity 的基本应用.
    /// </summary>
    public void EntityBaseApp()
    {
        #region  直接插入一条数据.
        BP.Port.Emp emp = new BP.Port.Emp();
        emp.CheckPhysicsTable(); 
        /*  检查物理表是否与Map一致 
         *  1，如果没有这个物理表则创建。
         *  2，如果缺少字段则创建。
         *  3，如果字段类型不一直则删除创建，比如原来是int类型现在map修改成string类型。
         *  4，map字段减少则不处理。
         *  5，手工的向物理表中增加的字段则不处理。
         *  6，数据源是视图字段不匹配则创建失败。
         * */
        emp.No = "zhangsan";
        emp.Name = "张三";
        emp.FK_Dept = "01";
        emp.Pass = "pub";
        emp.Insert();  // 如果主键重复要抛异常。
        #endregion  直接插入一条数据.

        #region  保存的方式插入一条数据.
        emp = new BP.Port.Emp();
        emp.No = "zhangsan";
        emp.Name = "张三";
        emp.FK_Dept = "01";
        emp.Pass = "pub";
        emp.Save();  // 如果主键重复直接更新，不会抛出异常。
        #endregion  保存的方式插入一条数据.

        #region  数据复制.
        /*
         * 如果一个实体与另外的一个实体两者的属性大致相同，就可以执行copy.
         *  比如：在创建人员时，张三与李四两者只是编号与名称不同，只是改变不同的属性就可以执行相关的业务操作。
         */
        Emp emp1 = new BP.Port.Emp("zhangsan");
        emp = new BP.Port.Emp();
        emp.Copy(emp1); // 同实体copy, 不同的实体也可以实现copy.
        emp.No = "lisi";
        emp.Name = "李四";
        emp.Insert();
        // copy 在业务逻辑上会经常应用，比如: 在一个流程中A节点表单与B节点表单字段大致相同，ccflow就是采用的copy方式处理。
        #endregion  数据复制.

        #region 查询.
        string msg = "";     // 查询这条数据.
        BP.Port.Emp myEmp = new BP.Port.Emp();
        myEmp.No = "zhangsan";
        if (myEmp.RetrieveFromDBSources() == 0)  // RetrieveFromDBSources() 返回来的是查询数量.
        {
            this.Response.Write("没有查询到编号等于zhangsan的人员记录.");
            return;
        }
        else
        {
            msg = "";
            msg += "<BR>编号:" + myEmp.No;
            msg += "<BR>名称:" + myEmp.Name;
            msg += "<BR>密码:" + myEmp.Pass;
            msg += "<BR>部门编号:" + myEmp.FK_Dept;
            msg += "<BR>部门名称:" + myEmp.FK_DeptText;
            this.Response.Write(msg);
        }

        myEmp = new BP.Port.Emp();
        myEmp.No = "zhangsan";
        myEmp.Retrieve(); // 执行查询，如果查询不到则要抛出异常。

        msg = "";
        msg += "<BR>编号:" + myEmp.No;
        msg += "<BR>名称:" + myEmp.Name;
        msg += "<BR>密码:" + myEmp.Pass;
        msg += "<BR>部门编号:" + myEmp.FK_Dept;
        msg += "<BR>部门名称:" + myEmp.FK_DeptText;
        this.Response.Write(msg);
        #endregion 查询.

        #region 两种方式的删除。
        // 删除操作。
        emp = new BP.Port.Emp();
        emp.No = "zhangsan";
        int delNum = emp.Delete(); // 执行删除。
        if (delNum == 0)
            this.Response.Write("删除zhangsan失败.");

        if (delNum == 1)
            this.Response.Write("删除zhangsan 成功..");
        if (delNum > 1)
            this.Response.Write("不应该出现的异常。");
        // 初试化实例后，执行删除，这种方式要执行两个sql.
        emp = new BP.Port.Emp("abc");
        emp.Delete();
        #endregion 两种方式的删除。

        #region 更新。
        emp = new BP.Port.Emp("zhangyifan"); // 事例化它.
        emp.Name = "张一帆123"; //改变属性.
        emp.Update();   // 更新它，这个时间BP将会把所有的属性都要执行更新，UPDATA 语句涉及到各个列。

        emp = new BP.Port.Emp("fuhui"); // 事例化它.
        emp.Update("Name", "福慧123");   //仅仅更新这一个属性。.UPDATA 语句涉及到Name列。
        #endregion 更新。
    }
     /// <summary>
    /// Entities 的基本应用.
    /// </summary>
    public void EntitiesBaseApp()
    {
        #region 查询全部
        /* 查询全部分为两种方式，1 从缓存里查询全部。2，从数据库查询全部。  */
        Emps emps = new Emps();
        int num = emps.RetrieveAll(); //从缓存里查询全部数据.

        this.Response.Write("RetrieveAll查询出来(" + num + ")个");
        foreach (Emp emp in emps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }
        //把entities 数据转入到DataTable里。
        DataTable empsDTfield = emps.ToDataTableField(); //以英文字段做为列名。
        DataTable empsDTDesc = emps.ToDataTableDesc(); //以中文字段做为列名。
        // 从数据库里查询全部。
        emps = new Emps();
        num = emps.RetrieveAllFromDBSource();
        this.Response.Write("RetrieveAllFromDBSource查询出来(" + num + ")个");
        foreach (Emp emp in emps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }
        #endregion 查询全部

        #region 按条件查询
        // 单个条件查询。
        Emps myEmps = new Emps();
        QueryObject qo = new QueryObject(myEmps);
        qo.AddWhere(EmpAttr.FK_Dept, "01");
        qo.addOrderBy(EmpAttr.No); // 增加排序规则,Order  OrderByDesc, addOrderByDesc addOrderByRandom. 
        num = qo.DoQuery();  // 返回查询的个数.
        DataTable mydt = qo.DoQueryToTable();  // 查询出来的数据转入到datatable里。.
        this.Response.Write("查询出来(" + num + ")个，部门编号=01的人员。");
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }

        Emps myEmp1s = new Emps();
        myEmp1s.Retrieve(EmpAttr.FK_Dept, "01");
        foreach (Emp  item in myEmp1s)
        {
            this.Response.Write("<hr>人员名称:" + item.Name);
            this.Response.Write("<br>人员编号:" + item.No);
            this.Response.Write("<br>部门编号:" + item.FK_Dept);
            this.Response.Write("<br>部门名称:" + item.FK_DeptText);
        }



        // 多个条件查询。
        myEmps = new Emps();
        qo = new QueryObject(myEmps);
        qo.AddWhere(EmpAttr.FK_Dept, "01");
        qo.addAnd();
        qo.AddWhere(EmpAttr.No, "guobaogen");
        num = qo.DoQuery();  // 返回查询的个数.
        this.Response.Write("查询出来(" + num + ")个，部门编号=01并且编号=guobaogen的人员。");
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }
        // 具有括号表达式的查询。
        myEmps = new Emps();
        qo = new QueryObject(myEmps);
        qo.addLeftBracket(); // 加上左括号.
        qo.AddWhere(EmpAttr.FK_Dept, "01");
        qo.addAnd();
        qo.AddWhere(EmpAttr.No, "guobaogen");
        qo.addRightBracket();  // 加上右括号.
        num = qo.DoQuery();  // 返回查询的个数.
        this.Response.Write("查询出来(" + num + ")个，部门编号=01并且编号=guobaogen的人员。");
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }
        // 具有where in 方式的查询。
        myEmps = new Emps();
        qo = new QueryObject(myEmps);
        qo.AddWhereInSQL(EmpAttr.No, "SELECT No FROM Port_Emp WHERE FK_Dept='02'");
        num = qo.DoQuery();  // 返回查询的个数.
        this.Response.Write("查询出来(" + num + ")个，WHERE IN (SELECT No FROM Port_Emp WHERE FK_Dept='02')人员。");
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }

        // 具有LIKE 方式的查询。
        myEmps = new Emps();
        qo = new QueryObject(myEmps);
        qo.AddWhere(EmpAttr.No, " LIKE ", "guo");
        num = qo.DoQuery();  // 返回查询的个数.
        this.Response.Write("查询出来(" + num + ")个，人员编号包含guo的人员。");
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }
        #endregion 按条件查询

        #region 集合业务处理.
        myEmps = new Emps();
        myEmps.RetrieveAll(); // 查询全部出来.
        // 遍历集合是常用的处理方法。
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }
        // 判断是否包含是指定的主键值.
        bool isHave = myEmps.Contains("Name", "郭宝庚"); //判断是否集合里面包含Name=郭宝庚的实体.
        bool isHave1 = myEmps.Contains("guobaogeng"); //判断是否集合里面主键No=guobaogeng的实体.

        // 获取Name=郭宝庚的实体，如果没有就返回空。
        Emp empFind = myEmps.GetEntityByKey("Name", "郭宝庚") as Emp;
        if (empFind == null)
            this.Response.Write("<br>没有找到: Name =郭宝庚 的实体.");
        else
            this.Response.Write("<br>已经找到了: Name =郭宝庚 的实体. 他的部门编号="+empFind.FK_Dept+"，部门名称="+empFind.FK_DeptText);
        // 批量更新实体。
        myEmps.Update(); // 等同于下一个循环。
        foreach (Emp emp in myEmps)
            emp.Update();
        // 删除实体.
        myEmps.Delete(); // 等同于下一个循环。
        foreach (Emp emp in myEmps)
        {
            emp.Delete();
        }
        // 执行数据库删除，类于执行 DELETE Port_Emp WHERE FK_Dept='01' 的sql.
        myEmps.Delete("FK_Dept", "01");
        #endregion
    }
}

 