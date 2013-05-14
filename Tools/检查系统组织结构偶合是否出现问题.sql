
/*
  用来检查您的数据偶合是否出现问题，如果没有查询到任何数据，就有可能您配置成功了。
  已经完全的把组织结构耦合到ccflow里面了.
*/
 
-- 检查人员中的部门是否对应的上。
SELECT * FROM Port_Emp WHERE FK_Dept NOT IN (SELECT No FROM PORT_DEPT)


-- 检查Port_Station 岗位分类是否对应的上.
SELECT * FROM Port_Station WHERE StaGrade NOT IN (SELECT IntKey FROM Sys_Enum WHERE EnumKey='StaGrade')


--检查人员与岗位对应关系表中的人员，是否有问题.
SELECT * FROM Port_EmpStation WHERE FK_Emp NOT IN (SELECT No FROM PORT_Emp)

--检查人员与岗位对应关系表中的岗位，是否有问题.
SELECT * FROM Port_EmpStation WHERE FK_Station NOT IN (SELECT No FROM Port_Station)


--检查人员与部门对应关系表中的人员，是否有问题.
SELECT * FROM Port_EmpStation WHERE FK_Emp NOT IN (SELECT No FROM PORT_Emp)

--检查人员与部门对应关系表中的部门，是否有问题.
SELECT * FROM Port_EmpDept WHERE FK_Dept NOT IN (SELECT No FROM Port_Dept)

 



 

