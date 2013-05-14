DELETE FROM Port_Dept @GO
DELETE FROM Port_Station @GO
DELETE FROM Port_Emp @GO
DELETE FROM Port_EmpStation @GO
DELETE FROM Port_EmpDept @GO


-- Port_Dept @GO
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('1','XXX市局','1') @GO
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('02','市征管科','1') @GO
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('03','市信息科','1') @GO

INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('04','YYY区局','1') @GO
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('05','YYY区征管科','1') @GO
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('06','YYY区信息科','1') @GO
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('07','YYY区一分局','1') @GO
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('08','YYY区二分局','1') @GO

INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('09','ZZZ区局','1') @GO
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('10','ZZZ区征管科','1') @GO
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('11','ZZZ区信息科','1') @GO
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('12','YYY区一分局','1') @GO
INSERT INTO Port_Dept (No,Name,ParentNo) VALUES('13','YYY区二分局','1') @GO

-- Port_Station @GO
INSERT INTO Port_Station (No,Name,StaGrade) VALUES('01','市局长','1') @GO
INSERT INTO Port_Station (No,Name,StaGrade) VALUES('02','市副局长','1') @GO

INSERT INTO Port_Station (No,Name,StaGrade) VALUES('03','市征管科长','2') @GO
INSERT INTO Port_Station (No,Name,StaGrade) VALUES('04','市征管科员','2') @GO
INSERT INTO Port_Station (No,Name,StaGrade) VALUES('05','市信息科长','2') @GO
INSERT INTO Port_Station (No,Name,StaGrade) VALUES('06','市信息科员','2') @GO

INSERT INTO Port_Station (No,Name,StaGrade) VALUES('11','区县局长','2') @GO
INSERT INTO Port_Station (No,Name,StaGrade) VALUES('12','区县副局长','2') @GO
INSERT INTO Port_Station (No,Name,StaGrade) VALUES('13','区县管科长','2') @GO
INSERT INTO Port_Station (No,Name,StaGrade) VALUES('14','区县管科员','2') @GO
INSERT INTO Port_Station (No,Name,StaGrade) VALUES('15','区县息科长','2') @GO
INSERT INTO Port_Station (No,Name,StaGrade) VALUES('16','区县息科员','2') @GO

INSERT INTO Port_Station (No,Name,StaGrade) VALUES('21','分局局长','3') @GO
INSERT INTO Port_Station (No,Name,StaGrade) VALUES('22','分局科员','3') @GO

-- Port_Emp @GO
-- 总经理部 @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('admin','admin','pub','01') @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('zhoupeng','周朋','pub','01') @GO

-- 市场部 @GO 
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('zhanghaicheng','张海成','pub','0101')  @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('zhangyifan','张一帆','pub','0101')  @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('zhoushengyu','周升雨','pub','0101')  @GO

-- 研发部 @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('guoxiangbin','郭祥斌','pub','0102') @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('zhoutianjiao','周天娇','pub','0102') @GO

-- 技术服务部经理 @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('qifenglin','祁凤林','pub','0103') @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('fuhui','福惠','pub','0103') @GO

-- 财务部 @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('yangyilei','杨依雷','pub','0104') @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('guobaogeng','郭宝庚','pub','0104') @GO

-- 人力资源部  @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('liping','李萍','pub','0105') @GO
INSERT INTO Port_Emp (No,Name,Pass,FK_Dept) VALUES('liyan','李言','pub','0105') @GO

 
-- Port_EmpDept 人员与部门的对应 @GO
INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('zhoupeng','01') @GO

INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('zhanghaicheng','0101') @GO
INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('zhangyifan','0101') @GO
INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('zhoushengyu','0101') @GO

INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('guoxiangbin','0102') @GO
INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('zhoutianjiao','0102') @GO

INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('qifenglin','0103') @GO
INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('fuhui','0103') @GO

INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('yangyilei','0104') @GO
INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('guobaogeng','0104') @GO

INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('liping','0105') @GO
INSERT INTO Port_EmpDept (FK_Emp,FK_Dept) VALUES('liyan','0105') @GO

-- Port_EmpStation 人员与岗位的对应 @GO
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('zhoupeng','01') @GO

INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('zhanghaicheng','02')  @GO
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('zhangyifan','07')  @GO
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('zhoushengyu','07')  @GO


INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('guoxiangbin','03') @GO
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('zhoutianjiao','08') @GO

INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('qifenglin','04') @GO
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('fuhui','09') @GO

INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('yangyilei','05') @GO
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('guobaogeng','10') @GO

INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('liping','06') @GO
INSERT INTO Port_EmpStation (FK_Emp,FK_Station) VALUES('liyan','11') @GO

-- WF_FlowSort   @GO
INSERT INTO WF_FlowSort (No,Name) VALUES('01','业务类')  @GO



