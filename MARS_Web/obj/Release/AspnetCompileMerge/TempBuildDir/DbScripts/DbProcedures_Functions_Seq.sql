
CREATE SEQUENCE  "GEN_MARS_5"."LOGREPORT_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 41701 CACHE 20 NOORDER  NOCYCLE ;
/
CREATE SEQUENCE  "GEN_MARS_5"."SEQ_DELETELOG"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 221 CACHE 20 NOORDER  NOCYCLE ;
/
CREATE SEQUENCE  "GEN_MARS_5"."TBLFEEDPROCESS_SEQ"  MINVALUE 0 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 880 CACHE 20 NOORDER  NOCYCLE;
/
CREATE SEQUENCE  "GEN_MARS_5"."TBLFEEDPROCESSDETAILS_SEQ"  MINVALUE 0 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 880 CACHE 20 NOORDER  NOCYCLE ;
/
CREATE SEQUENCE  "GEN_MARS_5"."TBLLOGREPORT_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 56961 CACHE 20 NOORDER  NOCYCLE ;
/
CREATE SEQUENCE  "GEN_MARS_5"."TBLSTGSTORYBOARD_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 20000 CACHE 20 NOORDER  NOCYCLE ;
/
CREATE SEQUENCE  "GEN_MARS_5"."TEMP1_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 1761 CACHE 20 NOORDER  NOCYCLE ;
/
CREATE SEQUENCE  "GEN_MARS_5"."TEMPDWTESTCASE_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 961 CACHE 20 NOORDER  NOCYCLE ;
/
CREATE SEQUENCE  "GEN_MARS_5"."TEMPSTGTESTCASE_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 961 CACHE 20 NOORDER  NOCYCLE ;
/
create or replace PROCEDURE "DeleteTestCase" (
TestCaseId IN number
)
IS
DelCount number;
Begin


select count(*) into DelCount from  T_TEST_REPORT_STEPS WHERE STEPS_ID IN ( SELECT STEPS_ID FROM T_TEST_STEPS WHERE TEST_CASE_ID = TestCaseId);
insert into deletelog select  SEQ_DELETELOG.nextval,'T_TEST_REPORT_STEPS','DELETE T_TEST_REPORT_STEPS, ' || DelCount,TestCaseId,'TestCaseId',SYSTIMESTAMP from dual;

Delete T_TEST_REPORT_STEPS WHERE STEPS_ID IN ( SELECT STEPS_ID FROM T_TEST_STEPS WHERE TEST_CASE_ID = TestCaseId);




select count(*) into DelCount from  T_TEST_REPORT where TEST_CASE_ID = TestCaseId;
insert into deletelog select  SEQ_DELETELOG.nextval,'T_TEST_REPORT','DELETE T_TEST_REPORT, ' || DelCount,TestCaseId,'TestCaseId',SYSTIMESTAMP from dual;
Delete T_TEST_REPORT where TEST_CASE_ID = TestCaseId;

select count(*) into DelCount from  T_PROJ_TEST_RESULT where TEST_CASE_ID = TestCaseId;
insert into deletelog select  SEQ_DELETELOG.nextval,'T_PROJ_TEST_RESULT','DELETE T_PROJ_TEST_RESULT, ' || DelCount,TestCaseId,'TestCaseId',SYSTIMESTAMP from dual;
Delete T_PROJ_TEST_RESULT where TEST_CASE_ID = TestCaseId;

select count(*) into DelCount from  TEST_DATA_SETTING where STEPS_ID in ( SELECT STEPS_ID FROM T_TEST_STEPS WHERE TEST_CASE_ID = TestCaseId);
insert into deletelog select  SEQ_DELETELOG.nextval,'TEST_DATA_SETTING','DELETE TEST_DATA_SETTING, ' || DelCount,TestCaseId,'TestCaseId',SYSTIMESTAMP from dual;
Delete TEST_DATA_SETTING where STEPS_ID in ( SELECT STEPS_ID FROM T_TEST_STEPS WHERE TEST_CASE_ID = TestCaseId);

select count(*) into DelCount from  REL_TC_DATA_SUMMARY where TEST_CASE_ID = TestCaseId;
insert into deletelog select  SEQ_DELETELOG.nextval,'REL_TC_DATA_SUMMARY','DELETE REL_TC_DATA_SUMMARY, ' || DelCount,TestCaseId,'TestCaseId',SYSTIMESTAMP from dual;
Delete REL_TC_DATA_SUMMARY where TEST_CASE_ID = TestCaseId;

select count(*) into DelCount from  T_TEST_STEPS where TEST_CASE_ID = TestCaseId;
insert into deletelog select  SEQ_DELETELOG.nextval,'T_TEST_STEPS','DELETE T_TEST_STEPS, ' || DelCount,TestCaseId,'TestCaseId',SYSTIMESTAMP from dual;
Delete T_TEST_STEPS where TEST_CASE_ID = TestCaseId;

select count(*) into DelCount from  REL_TEST_CASE_TEST_SUITE where TEST_CASE_ID = TestCaseId;
insert into deletelog select  SEQ_DELETELOG.nextval,'REL_TEST_CASE_TEST_SUITE','DELETE REL_TEST_CASE_TEST_SUITE, ' || DelCount,TestCaseId,'TestCaseId',SYSTIMESTAMP from dual;
Delete REL_TEST_CASE_TEST_SUITE where TEST_CASE_ID = TestCaseId;

select count(*) into DelCount from  T_STORYBOARD_DATASET_SETTING where STORYBOARD_DETAIL_ID in (select STORYBOARD_DETAIL_ID from T_PROJ_TC_MGR where TEST_CASE_ID = TestCaseId);
insert into deletelog select  SEQ_DELETELOG.nextval,'T_STORYBOARD_DATASET_SETTING','DELETE T_STORYBOARD_DATASET_SETTING, ' || DelCount,TestCaseId,'TestCaseId',SYSTIMESTAMP from dual;
Delete T_STORYBOARD_DATASET_SETTING where STORYBOARD_DETAIL_ID in (select STORYBOARD_DETAIL_ID from T_PROJ_TC_MGR where TEST_CASE_ID = TestCaseId);

select count(*) into DelCount from  T_PROJ_TC_MGR where TEST_CASE_ID = TestCaseId;
insert into deletelog select  SEQ_DELETELOG.nextval,'T_PROJ_TC_MGR','DELETE T_PROJ_TC_MGR, ' || DelCount,TestCaseId,'TestCaseId',SYSTIMESTAMP from dual;
Delete T_PROJ_TC_MGR where TEST_CASE_ID = TestCaseId;

select count(*) into DelCount from  REL_APP_TESTCASE where TEST_CASE_ID = TestCaseId;
insert into deletelog select  SEQ_DELETELOG.nextval,'REL_APP_TESTCASE','DELETE REL_APP_TESTCASE, ' || DelCount,TestCaseId,'TestCaseId',SYSTIMESTAMP from dual;
Delete REL_APP_TESTCASE where TEST_CASE_ID = TestCaseId;

select count(*) into DelCount from  T_TEST_CASE_SUMMARY where TEST_CASE_ID = TestCaseId;
insert into deletelog select  SEQ_DELETELOG.nextval,'T_TEST_CASE_SUMMARY','DELETE T_TEST_CASE_SUMMARY, ' || DelCount,TestCaseId,'TestCaseId',SYSTIMESTAMP from dual;
Delete T_TEST_CASE_SUMMARY where TEST_CASE_ID = TestCaseId;

End;
/

create or replace PROCEDURE Remove_Duplicate_Objects 
Is
OutputValue number :=0;
BEGIN
begin

-- select * from OBJECTS_ORIGINAL
-- select * from OBJECTS_DUPLICATE
--=========================================
-- Get All Original Objects 
--=========================================
INSERT INTO OBJECTS_ORIGINAL
SELECT OBJECT_NAME_ID, OBJECT_HAPPY_NAME
FROM T_OBJECT_NAMEINFO org
WHERE org.OBJECT_HAPPY_NAME IN (
  SELECT DISTINCT OBJECT_HAPPY_NAME
  FROM (
    select OBJECT_HAPPY_NAME, count(1)
    FROM T_OBJECT_NAMEINFO obj
    GROUP BY OBJECT_HAPPY_NAME
    HAVING COUNT(1) > 1
  )xy 
)
  AND org.OBJECT_NAME_ID IN (
    SELECT MIN(OBJECT_NAME_ID)
    FROM T_OBJECT_NAMEINFO xy
    WHERE org.OBJECT_HAPPY_NAME = xy.OBJECT_HAPPY_NAME
  );
commit;
--=========================================
-- Get All Duplicate Objects 
--=========================================
INSERT INTO OBJECTS_DUPLICATE
SELECT OBJECT_NAME_ID, OBJECT_HAPPY_NAME
FROM T_OBJECT_NAMEINFO org
WHERE org.OBJECT_HAPPY_NAME IN (
  SELECT DISTINCT OBJECT_HAPPY_NAME
  FROM (
    select OBJECT_HAPPY_NAME, count(1)
    FROM T_OBJECT_NAMEINFO obj
    GROUP BY OBJECT_HAPPY_NAME
    HAVING COUNT(1) > 1
  )xy 
)
  AND org.OBJECT_NAME_ID NOT IN (
    SELECT MIN(OBJECT_NAME_ID)
    FROM T_OBJECT_NAMEINFO xy
    WHERE org.OBJECT_HAPPY_NAME = xy.OBJECT_HAPPY_NAME
  );

commit;

--==============================================
-- Update Duplicate Object reference with Original
--==============================================
DECLARE
  CURSOR OBJECTSDUPLICATE
  IS 
  SELECT OBEJCT_HAPPY_NAME, OBJECT_NAME_ID
  FROM OBJECTS_DUPLICATE;
BEGIN

  FOR OBJECTS_EACH IN OBJECTSDUPLICATE
  LOOP

    DBMS_OUTPUT.PUT_LINE(OBJECTS_EACH.OBJECT_NAME_ID);

    MERGE INTO T_REGISTED_OBJECT trou
    USING (
      SELECT tro.OBJECT_ID, oo.OBJECT_NAME_ID
      FROM T_REGISTED_OBJECT tro
      INNER JOIN T_OBJECT_NAMEINFO objn ON objn.OBJECT_NAME_ID = tro.OBJECT_NAME_ID
        AND objn.OBJECT_NAME_ID = OBJECTS_EACH.OBJECT_NAME_ID
      INNER JOIN OBJECTS_ORIGINAL oo ON oo.OBEJCT_HAPPY_NAME = objn.OBJECT_HAPPY_NAME
      WHERE tro.OBJECT_NAME_ID = OBJECTS_EACH.OBJECT_NAME_ID
    ) UPD
    ON (UPD.OBJECT_ID = trou.OBJECT_ID)
    WHEN MATCHED THEN 
    UPDATE SET
      trou.OBJECT_NAME_ID = UPD.OBJECT_NAME_ID;

  END LOOP;

END;

  COMMIT;

--==============================================
-- Remove Duplicate Objects
--==============================================
DELETE
FROM T_OBJECT_NAMEINFO org
WHERE org.OBJECT_HAPPY_NAME IN (
  SELECT DISTINCT OBJECT_HAPPY_NAME
  FROM (
    select OBJECT_HAPPY_NAME, count(1)
    FROM T_OBJECT_NAMEINFO obj
    GROUP BY OBJECT_HAPPY_NAME
    HAVING COUNT(1) > 1
  )xy 
)
  AND org.OBJECT_NAME_ID NOT IN (
    SELECT MIN(OBJECT_NAME_ID)
    FROM T_OBJECT_NAMEINFO xy
    WHERE org.OBJECT_HAPPY_NAME = xy.OBJECT_HAPPY_NAME
  );

  COMMIT;
  OutputValue  :=3;
End;
End Remove_Duplicate_Objects;
/

create or replace PROCEDURE          "SP_EXPORT_COMPAREPARAM" (
sl_cursor OUT SYS_REFCURSOR)
IS

stm VARCHAR2(30000);

BEGIN
    stm := 'SELECT DATA_SOURCE_NAME as "Data Source Name",
                   DATA_SOURCE_TYPE as "Data Source Type",
                   DETAILS as "Details "
            FROM T_DATA_SOURCE ORDER BY 1';
            --edited by foram shah 25/3/19
    OPEN sl_cursor FOR  stm ;
END;
/

create or replace PROCEDURE          "SP_EXPORT_EXPORTOBJECT" (
APPLICATION IN VARCHAR2,
sl_cursor OUT SYS_REFCURSOR
)
IS

	stm VARCHAR2(30000);
	NEWAPPLICATION_ID NUMBER;
	TOTALCOUNT NUMBER:=0;
	MAXCOUNT NUMBER:=0;

BEGIN
	SELECT COUNT(*) INTO TOTALCOUNT from T_REGISTED_OBJECT WHERE APPLICATION_ID=NEWAPPLICATION_ID;

	SELECT MAX(ID)+1 INTO MAXCOUNT FROM LOGREPORT; 
	--INSERT INTO LOGREPORT VALUES('OBJECT',NULL,NULL,'TOTAL-COUNT'|| TOTALCOUNT,MAXCOUNT,(SELECT SYSDATE FROM DUAL),NULL);

    SELECT APPLICATION_ID INTO NEWAPPLICATION_ID FROM T_REGISTERED_APPS WHERE UPPER(APP_SHORT_NAME)=UPPER(APPLICATION) AND ROWNUM=1; -- TO GET THE NEXT VALUE
    stm := 'SELECT ni.object_happy_name as "OBJECT NAME",
                   typed.TYPE_NAME as "TYPE", 
                   o.QUICK_ACCESS,
                   o.OBJECT_TYPE as "PARENT",
                   o."COMMENT",
                   o.ENUM_TYPE,
                   '''' as SQL
            FROM T_OBJECT_NAMEINFO ni
            INNER JOIN T_REGISTED_OBJECT o ON o.OBJECT_NAME_ID = ni.OBJECT_NAME_ID
            INNER JOIN T_GUI_COMPONENT_TYPE_DIC typed ON o.TYPE_ID=typed.TYPE_ID
            WHERE APPLICATION_ID=' || NEWAPPLICATION_ID || '
                AND EXISTS (
                    SELECT 1
                    FROM t_object_nameinfo tobjparent
                    WHERE tobjparent.object_happy_name = o.object_type
                )
            ORDER BY 4, CASE typed.TYPE_NAME WHEN ''Pegwindow'' THEN 1 ELSE 2 END, 1';
    OPEN sl_cursor FOR  stm ;
    --execute IMMEDIATE stm;
--edited by foram shah 25/3/19



END SP_EXPORT_EXPORTOBJECT;
/

create or replace PROCEDURE "SP_EXPORT_LOGREPORT" (
FEEDPROCESSDETAILID IN NUMBER,
--FILENAME IN VARCHAR2,
sl_cursor OUT SYS_REFCURSOR)
IS

stm VARCHAR2(30000);
BEGIN
    stm := 'select TO_CHAR(tblLOGREPORT.CREATIONDATE,''MM/DD/YYYY hh:mm:ss'') as "TimeStamp",
tblLOGREPORT.MESSAGETYPE as "Message Type",
tblLOGREPORT.ACTION,
tblLOGREPORT.CELLADDRESS aS "SpreadSheet cell Address",
tblLOGREPORT.VALIDATIONNAME as "Validation Name",
tblLOGREPORT.VALIDATIONDESCRIPTION as "Validation Fail Description",
tblLOGREPORT.APPLICATIONNAME as "Application Name",
tblLOGREPORT.PROJECTNAME as "Project Name",
tblLOGREPORT.STORYBOARDNAME as "Storyboard Name",
tblLOGREPORT.TESTSUITE as "Test Suite Name",

tblLOGREPORT.TESTCASENAME aS "TestCase Name",
tblLOGREPORT.TESTSTEPNUMBER AS "Test step Number",

tblLOGREPORT.DATASETNAME AS "Dataset Name",
tblLOGREPORT.DEPENDENCY as "Dependancy",
tblLOGREPORT.RUNORDER as "Run Order",
tblLOGREPORT.OBJECTNAME AS "Object Name",
tblLOGREPORT.COMMENTDATA AS "Comment",
tblLOGREPORT.ERRORDESCRIPTION AS "Error Description",
tblLOGREPORT.PROGRAMLOCATION AS "Program Location" 
, case when tblLOGREPORT.STORYBOARDNAME is null then tblLOGREPORT.TESTCASENAME
else tblLOGREPORT.STORYBOARDNAME end as "Tab Name"
 ,tblLOGREPORT.general as "General"
    from tblLOGREPORT 
    INNER JOIN tblfeedprocessdetails ON tblfeedprocessdetails.feedprocessid = tbllogreport.feedprocessid
    where 
         tblfeedprocessdetails.feedprocessid= ' || FEEDPROCESSDETAILID || ' ORDER BY 9, 10, 1';
    OPEN sl_cursor FOR  stm ;
END;
-- upper(LOGREPORT.FILENAME) = upper(''' || FILENAME || ''')
/

create or replace PROCEDURE   "SP_EXPORT_STORYBOARD" (
PROJECT IN VARCHAR2,
sl_cursor OUT SYS_REFCURSOR)
IS


stm VARCHAR2(30000);

BEGIN
    stm := '
    select RunOrder,ApplicationName,ProjectName,ProjectDescription,storyboard_name,ActionName,StepName,SuiteName,CaseName,DataSetName,Dependency from (
select 
ROW_NUMBER() OVER (PARTITION BY RunOrder,ApplicationName,ProjectName,ProjectDescription,storyboard_name,ActionName,StepName ORDER BY RunOrder DESC ) as lRowId
,RunOrder,ApplicationName,ProjectName,ProjectDescription,storyboard_name,ActionName,StepName,SuiteName,CaseName,DataSetName,Dependency from (
select  LISTAGG(ApplicationName, '','') WITHIN GROUP (ORDER BY ApplicationName) as ApplicationName,RunOrder,ProjectName,ProjectDescription,storyboard_name,ActionName
,StepName, SuiteName,CaseName, DataSetName, Dependency  from (
SELECT distinct
	relprjtc.run_order AS RunOrder,
	app.app_short_name AS ApplicationName ,
	prj.project_name AS ProjectName,
	prj.project_description AS ProjectDescription,
	storyboard.storyboard_name ,
	lkp.display_name AS ActionName,
	relprjtc.alias_name AS StepName,	
	CASE 
		WHEN relcase.test_suite_id is  null then ''''
		ELSE suits.test_suite_name 
		END  
	AS SuiteName,  
	CASE 
		WHEN relcase.test_suite_id is  null then ''''
		ELSE cases.test_case_name
		END 
	AS CaseName,    
	dataset.alias_name as DataSetName,
	nvl(depends.alias_name,''None'') as Dependency
FROM T_STORYBOARD_SUMMARY storyboard
INNER JOIN t_test_project prj ON storyboard.assigned_project_id = prj.project_id
INNER JOIN REL_APP_PROJ relappprj ON relappprj.project_id = prj.project_id
INNER JOIN t_registered_apps app ON app.application_id = relappprj.application_id
INNER JOIN T_PROJ_TC_MGR relprjtc ON relprjtc.project_id  = prj.project_id 
	AND relprjtc.storyboard_id = storyboard.storyboard_id
INNER JOIN t_test_suite suits ON suits.test_suite_id = relprjtc.test_suite_id
INNER JOIN t_test_case_summary cases ON cases.test_case_id = relprjtc.test_case_id
left JOIN REL_TEST_CASE_TEST_SUITE  relcase ON relcase.test_suite_id = suits.test_suite_id 
	AND relcase.test_case_id = cases.test_case_id
INNER JOIN REL_TC_DATA_SUMMARY reldataset ON reldataset.test_case_id = cases.test_case_id
INNER JOIN T_STORYBOARD_DATASET_SETTING relsrt ON relsrt.storyboard_detail_id = relprjtc.storyboard_detail_id
INNER JOIN t_test_data_summary dataset ON dataset.data_summary_id = reldataset.data_summary_id 
	AND dataset.data_summary_id = relsrt.data_summary_id
INNER JOIN system_lookup lkp ON lkp.value = relprjtc.run_type 
	AND lkp.field_name = ''RUN_TYPE''
	AND lkp.TABLE_NAME=''T_PROJ_TC_MGR''
LEFT JOIN T_PROJ_TC_MGR  depends ON depends.storyboard_detail_id = relprjtc.depends_on
WHERE prj.project_name = '''|| PROJECT ||'''

ORDER BY storyboard.storyboard_name desc, relprjtc.run_order asc)
group by RunOrder,ProjectName,ProjectDescription,storyboard_name,ActionName
,StepName, SuiteName,CaseName, DataSetName, Dependency)
group by RunOrder,ApplicationName,ProjectName,ProjectDescription,storyboard_name,ActionName,StepName,SuiteName,CaseName,DataSetName,Dependency) where lrowid = 1
    ';
            --edited by foram shah 25/3/19
    OPEN sl_cursor FOR  stm ;
END;

/

create or replace PROCEDURE SP_EXPORT_TESTCASE( 
TESTSUITENAME IN VARCHAR2,
TESTCASENAME IN VARCHAR2,
sl_cursor OUT SYS_REFCURSOR
)
IS
stm VARCHAR2(30000);
BEGIN
    stm := '';
    stm := '

select  
DISTINCT
DBMS_LOB.SUBSTR("Application",4000,1) AS "Application"
,"test_suite_name","test_case_name","test_step_description","key_word_name","object_happy_name","parameter","COMMENT",
DBMS_LOB.SUBSTR("DATASETNAME",4000,1) AS "DATASETNAME",
DBMS_LOB.SUBSTR("DATASETDESCRIPTION",4000,1) AS "DATASETDESCRIPTION",
DBMS_LOB.SUBSTR("DATASETVALUE",4000,1) AS "DATASETVALUE",
DBMS_LOB.SUBSTR("SKIP",4000,1) AS "SKIP"
,"RUN_ORDER"
from (
        SELECT 
            (SELECT  concatenate_application(t.TEST_SUITE_ID) FROM DUAL) AS "Application",
            t.TEST_SUITE_NAME AS "test_suite_name",
            tc.TEST_CASE_NAME as "test_case_name",
            tc.TEST_STEP_DESCRIPTION AS "test_step_description",
            tk.KEY_WORD_NAME as "key_word_name",
            tobn.OBJECT_HAPPY_NAME AS "object_happy_name",
            ts.COLUMN_ROW_SETTING as "parameter",
            ts."COMMENT",
            ( SELECT concatenate_datasetnames(tc.TEST_CASE_ID) DATASETNAME FROM DUAL) "DATASETNAME",
            (SELECT concatenate_datasetdescription(tc.TEST_CASE_ID) FROM DUAL) "DATASETDESCRIPTION",
            (SELECT concatenate_teststepvalue(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "DATASETVALUE",
            (SELECT concatenate_teststepskip(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "SKIP",
            ts."RUN_ORDER"
          FROM T_TEST_SUITE t
           INNER JOIN REL_APP_TESTSUITE relta ON relta.TEST_SUITE_ID = t.TEST_SUITE_ID
          INNER JOIN T_REGISTERED_APPS tapp ON tapp.APPLICATION_ID = relta.APPLICATION_ID
          INNER JOIN REL_TEST_CASE_TEST_SUITE reltcts ON reltcts.TEST_SUITE_ID = t.TEST_SUITE_ID
          INNER JOIN REL_APP_TESTCASE reapp ON reapp.test_case_id = reltcts.TEST_CASE_ID
        and reapp.application_id= relta.application_id AND tapp.APPLICATION_ID = reapp.application_id          
          INNER JOIN T_TEST_CASE_SUMMARY tc ON tc.TEST_CASE_ID = reltcts.TEST_CASE_ID
          left JOIN T_TEST_STEPS ts ON ts.TEST_CASE_ID = tc.TEST_CASE_ID
          left JOIN T_KEYWORD tk ON tk.KEY_WORD_ID = ts.KEY_WORD_ID
         -- LEFT JOIN t_registed_object tob ON tob.object_id = ts.object_id
          LEFT JOIN T_OBJECT_NAMEINFO tobn ON tobn.OBJECT_NAME_ID = ts.OBJECT_NAME_ID
          WHERE t.TEST_SUITE_NAME='''||TESTSUITENAME||''' and tc.TEST_CASE_NAME = '''||TESTCASENAME||'''
           ) 
           ORDER BY 3 DESC,13';
    OPEN sl_cursor FOR  stm ; 

    INSERT INTO logreport(logreport.filename,logreport.object,logreport.status,logreport.logdetails,logreport.id,logreport.createdon,logreport.feedprocessdetailid)
    select 'Test Suite Name : ' || TESTSUITENAME AS filename,'' AS object,'SUCCESS','TEST CASE NAME :'|| t_test_case_summary.test_case_name || ' DATASET COUNT : ' || (SELECT COUNT(1) FROM rel_tc_data_summary  where rel_tc_data_summary.TEST_CASE_ID = T_TEST_CASE_SUMMARY.TEST_CASE_ID)|| ' DATASET COUNT : '||  (SELECT COUNT(1) FROM t_test_steps where t_test_steps.test_case_id = T_TEST_CASE_SUMMARY.TEST_CASE_ID),LOGREPORT_SEQ.NEXTVAL,(SELECT SYSDATE FROM DUAL),0 FROM t_test_suite  INNER JOIN REL_TEST_CASE_TEST_SUITE on rel_test_case_test_suite.TEST_SUITE_ID = t_test_suite.TEST_SUITE_ID INNER JOIN T_TEST_CASE_SUMMARY ON T_TEST_CASE_SUMMARY.TEST_CASE_ID = REL_TEST_CASE_TEST_SUITE.TEST_CASE_ID  where UPPER(test_suite_name) =  UPPER(TESTSUITENAME);

END;
/

create or replace PROCEDURE SP_EXPORT_TESTSUITE( 
TESTSUITENAME IN VARCHAR2,
sl_cursor OUT SYS_REFCURSOR
)
IS
stm VARCHAR2(30000);
BEGIN
    stm := '';
    stm := '

select  
DISTINCT
DBMS_LOB.SUBSTR("Application",4000,1) AS "Application"
,"test_suite_name","test_case_name","test_step_description","key_word_name","object_happy_name","parameter","COMMENT",
DBMS_LOB.SUBSTR("DATASETNAME",4000,1) AS "DATASETNAME",
DBMS_LOB.SUBSTR("DATASETDESCRIPTION",4000,1) AS "DATASETDESCRIPTION",
DBMS_LOB.SUBSTR("DATASETVALUE",4000,1) AS "DATASETVALUE",
DBMS_LOB.SUBSTR("SKIP",4000,1) AS "SKIP"
,"RUN_ORDER"
from (
        SELECT 
            (SELECT  concatenate_application(t.TEST_SUITE_ID) FROM DUAL) AS "Application",
            t.TEST_SUITE_NAME AS "test_suite_name",
            tc.TEST_CASE_NAME as "test_case_name",
            tc.TEST_STEP_DESCRIPTION AS "test_step_description",
            tk.KEY_WORD_NAME as "key_word_name",
            tobn.OBJECT_HAPPY_NAME AS "object_happy_name",
            ts.COLUMN_ROW_SETTING as "parameter",
            ts."COMMENT",
            ( SELECT concatenate_datasetnames(tc.TEST_CASE_ID) DATASETNAME FROM DUAL) "DATASETNAME",
            (SELECT concatenate_datasetdescription(tc.TEST_CASE_ID) FROM DUAL) "DATASETDESCRIPTION",
            (SELECT concatenate_teststepvalue(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "DATASETVALUE",
            (SELECT concatenate_teststepskip(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "SKIP",
            ts."RUN_ORDER"
          FROM T_TEST_SUITE t
           INNER JOIN REL_APP_TESTSUITE relta ON relta.TEST_SUITE_ID = t.TEST_SUITE_ID
          INNER JOIN T_REGISTERED_APPS tapp ON tapp.APPLICATION_ID = relta.APPLICATION_ID
          INNER JOIN REL_TEST_CASE_TEST_SUITE reltcts ON reltcts.TEST_SUITE_ID = t.TEST_SUITE_ID
          INNER JOIN REL_APP_TESTCASE reapp ON reapp.test_case_id = reltcts.TEST_CASE_ID
        and reapp.application_id= relta.application_id AND tapp.APPLICATION_ID = reapp.application_id          
          INNER JOIN T_TEST_CASE_SUMMARY tc ON tc.TEST_CASE_ID = reltcts.TEST_CASE_ID
          left JOIN T_TEST_STEPS ts ON ts.TEST_CASE_ID = tc.TEST_CASE_ID
          left JOIN T_KEYWORD tk ON tk.KEY_WORD_ID = ts.KEY_WORD_ID
         -- LEFT JOIN t_registed_object tob ON tob.object_id = ts.object_id
          LEFT JOIN T_OBJECT_NAMEINFO tobn ON tobn.OBJECT_NAME_ID = ts.OBJECT_NAME_ID
          WHERE t.TEST_SUITE_NAME='''||TESTSUITENAME||'''
           ) 
           ORDER BY 3 DESC,13';
    OPEN sl_cursor FOR  stm ; 

    INSERT INTO logreport(logreport.filename,logreport.object,logreport.status,logreport.logdetails,logreport.id,logreport.createdon,logreport.feedprocessdetailid)
    select 'Test Suite Name : ' || TESTSUITENAME AS filename,'' AS object,'SUCCESS','TEST CASE NAME :'|| t_test_case_summary.test_case_name || ' DATASET COUNT : ' || (SELECT COUNT(1) FROM rel_tc_data_summary  where rel_tc_data_summary.TEST_CASE_ID = T_TEST_CASE_SUMMARY.TEST_CASE_ID)|| ' DATASET COUNT : '||  (SELECT COUNT(1) FROM t_test_steps where t_test_steps.test_case_id = T_TEST_CASE_SUMMARY.TEST_CASE_ID),LOGREPORT_SEQ.NEXTVAL,(SELECT SYSDATE FROM DUAL),0 FROM t_test_suite  INNER JOIN REL_TEST_CASE_TEST_SUITE on rel_test_case_test_suite.TEST_SUITE_ID = t_test_suite.TEST_SUITE_ID INNER JOIN T_TEST_CASE_SUMMARY ON T_TEST_CASE_SUMMARY.TEST_CASE_ID = REL_TEST_CASE_TEST_SUITE.TEST_CASE_ID  where UPPER(test_suite_name) =  UPPER(TESTSUITENAME);

END;
/

create or replace PROCEDURE   "SP_EXPORT_VARIABLE" (
sl_cursor OUT SYS_REFCURSOR)
IS


stm VARCHAR2(30000);

BEGIN

stm := ' 
select Field_Name as Name
,display_name as Value
,table_name as Type 
, case when  status = 1 and table_name in (''MODAL_VAR'',''LOOP_VAR'') then ''BASELINE''
       when status = 2 and table_name in (''MODAL_VAR'',''LOOP_VAR'') then ''COMPARE''
       Else '''' End
AS "Base/Comp"
from system_lookup 

where 
table_name 
in 
(''MODAL_VAR'',''GLOBAL_VAR'',''LOOP_VAR'')
order by table_name

';

 OPEN sl_cursor FOR  stm ;
END;

/

create or replace PROCEDURE "SP_IMPORT_FILE_COMPAREPARAM" (
DATA_SOURCE_NAME VARCHAR2:=NULL,
DATA_SOURCE_TYPE NUMBER:=0,
DETAILS VARCHAR2:=NULL,
FEEDPROCESSDETAILID in number

)
IS
BEGIN
declare 
	COUNT_ID NUMBER:=0;
	NEWCOUNT_ID NUMBER:=0;
	CURRENTDATE TIMESTAMP;
	BEGIN
		SELECT  COUNT(*) INTO COUNT_ID FROM TBLSTGCOMPAREPARAM;

		SELECT SYSDATE INTO CURRENTDATE FROM DUAL;
		IF COUNT_ID=0 THEN
			BEGIN
			NEWCOUNT_ID:=1;
			END;
		ELSE
			BEGIN
				SELECT  MAX(ID)+1  INTO NEWCOUNT_ID FROM TBLSTGCOMPAREPARAM;
			END;
		END IF;

	INSERT INTO TBLSTGCOMPAREPARAM(DATASOURCENAME,DATASOURCETYPE,DETAILS,FEEDPROCESSDETAILID,ID,CREATEDON)
	VALUES(DATA_SOURCE_NAME,DATA_SOURCE_TYPE,DETAILS,FEEDPROCESSDETAILID,NEWCOUNT_ID,CURRENTDATE);
	END;
END SP_IMPORT_FILE_COMPAREPARAM;

/

create or replace PROCEDURE "SP_IMPORT_FILE_EXPORTTEST" ( sl_cursor OUT SYS_REFCURSOR)  
IS
    stm varchar(30000);
BEGIN
    stm := 'SELECT * from IMPORTTEST';
    OPEN sl_cursor FOR  stm ;

	 stm := 'SELECT * from IMPORTTEST';
    OPEN sl_cursor FOR  stm ;
END;
/

create or replace PROCEDURE "SP_IMPORT_FILE_EXPORTTESTCASE" ( sl_cursor OUT SYS_REFCURSOR)  
IS
    stm varchar(30000);
BEGIN
    stm := 'SELECT * from IMPORTMULTIEXCEL order by ROWNUMBER';
    OPEN sl_cursor FOR  stm ;


END;
/

create or replace PROCEDURE "SP_IMPORT_FILE_OBJECT" (
OBJECTNAME in varchar2:=NULL,
OBJECTTYPE in varchar2:=NULL,
QUICKACCESS in varchar2:=NULL,
PARENT in varchar2:=NULL,
OBJECTCOMMENT in varchar2:=NULL,
ENUMTYPE in varchar2:=NULL,
OBJECTSQL in varchar2:=NULL,
APPLICATIONNAME in varchar2 :=NULL,
FEEDPROCESSDETAILID in number
)

is

begin
declare 
	COUNT_ID NUMBER:=0;
	NEWCOUNT_ID NUMBER:=0;
	CURRENTDATE TIMESTAMP;

	BEGIN
		SELECT  COUNT(*) INTO COUNT_ID FROM TBLSTGGUIOBJECT;

		SELECT SYSDATE INTO CURRENTDATE FROM DUAL;
		IF COUNT_ID=0 THEN
			BEGIN
			NEWCOUNT_ID:=1;
			END;
		ELSE
			BEGIN
				SELECT  MAX(ID)+1  INTO NEWCOUNT_ID FROM TBLSTGGUIOBJECT;
			END;
		END IF;

  insert into TBLSTGGUIOBJECT(OBJECTNAME,TYPE,QUICKACCESS,PARENT,OBJECTCOMMENT,ENUMTYPE,SQL,APPLICATIONNAME,FEEDPROCESSDETAILID,ID,CREATEDON)
  values(OBJECTNAME,OBJECTTYPE,QUICKACCESS,PARENT,OBJECTCOMMENT,ENUMTYPE,OBJECTSQL,APPLICATIONNAME,FEEDPROCESSDETAILID,NEWCOUNT_ID,CURRENTDATE);
  END;

end SP_IMPORT_FILE_OBJECT;

/

create or replace PROCEDURE "SP_IMPORT_FILE_STORYBOARD" (
  RUNORDER in number:=null,
  APPLICATIONNAME in VARCHAR2:=null,
  PROJECTNAME in VARCHAR2:=null,
  PROJECTDETAIL in VARCHAR2:=null,
  STORYBOARDNAME in VARCHAR2:=null,
  ACTIONNAME in VARCHAR2:=null,
  STEPNAME in VARCHAR2:=null,
  SUITENAME in VARCHAR2:=null,
  CASENAME in VARCHAR2:=null ,
  DATASETNAME in  VARCHAR2:=null,
  DEPENDENCY in VARCHAR2:=null,
    
  FEEDPROCESSDETAILID IN number:=null,
  TABNAME IN VARCHAR2:=NULL  
)

is

begin
declare 
	COUNT_ID NUMBER:=0;
	NEWCOUNT_ID NUMBER:=0;
	CURRENTDATE TIMESTAMP;
	ROW_Order NUMBER:=0;
    FEEDPROCESSDETAIL_ID NUMBER:=0;
    BEGIN
		SELECT  COUNT(*) INTO COUNT_ID FROM TBLSTGSTORYBOARD;

		SELECT SYSDATE INTO CURRENTDATE FROM DUAL;
		IF COUNT_ID=0 THEN
			BEGIN
			NEWCOUNT_ID:=1;
			END;
		ELSE
			BEGIN
				SELECT  MAX(ID)+1  INTO NEWCOUNT_ID FROM TBLSTGSTORYBOARD;
			END;
		END IF;

SELECT TO_CHAR(RUNORDER)   into ROW_Order
FROM dual;

SELECT TO_CHAR(FEEDPROCESSDETAILID)  into FEEDPROCESSDETAIL_ID
FROM dual;

  insert into TBLSTGSTORYBOARD(ID, FEEDPROCESSDETAILID,TABNAME,RUNORDER,APPLICATIONNAME,PROJECTNAME,PROJECTDETAIL,STORYBOARDNAME,
  ACTIONNAME,STEPNAME,SUITENAME,CASENAME,DATASETNAME,DEPENDENCY)
  values(NEWCOUNT_ID,FEEDPROCESSDETAIL_ID,TABNAME,ROW_Order,APPLICATIONNAME,PROJECTNAME,PROJECTDETAIL,STORYBOARDNAME,
  ACTIONNAME,STEPNAME,SUITENAME,CASENAME,DATASETNAME,DEPENDENCY);

END;


end SP_IMPORT_FILE_STORYBOARD;
/

create or replace PROCEDURE "SP_IMPORT_FILE_TESTCASE" (
TESTSUITENAME in VARCHAR2:=null,
  TESTCASENAME in VARCHAR2:=null ,
  TESTCASEDESCRIPTION in VARCHAR2:=null,
  DATASETMODE in  VARCHAR2:=null,
  KEYWORD in VARCHAR2:=null,
  OBJECT in VARCHAR2:=null,
  PARAMETER in VARCHAR2:=null,
  COMMENTS in VARCHAR2:=null,
  DATASETNAME in VARCHAR2:=null,
  DATASETVALUE in VARCHAR2:=null,
  ROWNUMBER in number,
  FEEDPROCESSDETAILID IN NUMBER,
  TABNAME IN VARCHAR2:=NULL,
  APPLICATION IN VARCHAR2:=NULL,
  SKIP IN NUMBER,
  DATASETDESCRIPTION VARCHAR2:=NULL
)

is

begin
declare 
	COUNT_ID NUMBER:=0;
	NEWCOUNT_ID NUMBER:=0;
	CURRENTDATE TIMESTAMP;
	BEGIN
		SELECT  COUNT(*) INTO COUNT_ID FROM TBLSTGTESTCASE;

		SELECT SYSDATE INTO CURRENTDATE FROM DUAL;
		IF COUNT_ID=0 THEN
			BEGIN
			NEWCOUNT_ID:=1;
			END;
		ELSE
			BEGIN
				SELECT  MAX(ID)+1  INTO NEWCOUNT_ID FROM TBLSTGTESTCASE;
			END;
		END IF;


  insert into TBLSTGTESTCASE(TESTSUITENAME,TESTCASENAME,TESTCASEDESCRIPTION,DATASETMODE,KEYWORD,OBJECT,PARAMETER,COMMENTS,DATASETNAME,DATASETVALUE,ROWNUMBER,FEEDPROCESSDETAILID,TABNAME,APPLICATION,ID,CREATEDON,SKIP,DATASETDESCRIPTION)
  values(TESTSUITENAME,TESTCASENAME,TESTCASEDESCRIPTION,DATASETMODE,KEYWORD,OBJECT,PARAMETER,COMMENTS,DATASETNAME,DATASETVALUE,ROWNUMBER,FEEDPROCESSDETAILID,TABNAME,APPLICATION,NEWCOUNT_ID,CURRENTDATE,SKIP,DATASETDESCRIPTION);

	END;


end SP_IMPORT_FILE_TESTCASE;


/

create or replace PROCEDURE "SP_IMPORT_FILE_VARIABLE" (
  FEEDPROCESSDETAILID IN number:=null,
  NAME IN varchar2:=null,
  Value IN clob:=null,
  Type IN varchar2:=null,
  BASE_COMP IN varchar2:=null
)

is

begin
declare 
	COUNT_ID NUMBER:=0;
	NEWCOUNT_ID NUMBER:=0;
	CURRENTDATE TIMESTAMP;
	ROW_Order NUMBER:=0;
    FEEDPROCESSDETAIL_ID NUMBER:=0;
    BEGIN
		SELECT  COUNT(*) INTO COUNT_ID FROM tblstgvariable;

		SELECT SYSDATE INTO CURRENTDATE FROM DUAL;
		IF COUNT_ID=0 THEN
			BEGIN
			NEWCOUNT_ID:=1;
			END;
		ELSE
			BEGIN
				SELECT  MAX(ID)+1  INTO NEWCOUNT_ID FROM tblstgvariable;
			END;
		END IF;


SELECT TO_CHAR(FEEDPROCESSDETAILID)  into FEEDPROCESSDETAIL_ID
FROM dual;

  insert into tblstgvariable(ID,FEEDPROCESSDETAILID,NAME,VALUE,TYPE,BASE_COMP,CREATEDDATE,base_comp_id)
  values(NEWCOUNT_ID,FEEDPROCESSDETAIL_ID,upper(NAME),upper(VALUE),upper(TYPE),upper(BASE_COMP),CURRENTDATE, case when upper(BASE_COMP) = 'BASELINE' then 1 when upper(BASE_COMP) = 'COMPARE' then 2 else 0 end);

END;


end SP_IMPORT_FILE_VARIABLE;
/
create or replace PROCEDURE SP_TESTCASE_DETAILS( 
    TESTCASEID IN NUMBER,
    sl_cursor OUT SYS_REFCURSOR
)
IS
stm VARCHAR2(30000);
BEGIN

OPEN sl_cursor FOR
    SELECT DISTINCT
        DBMS_LOB.SUBSTR("Application",4000,1) AS "Application",
        "test_suite_name","test_case_name","test_step_description","key_word_name","object_happy_name","parameter","COMMENT",
        DBMS_LOB.SUBSTR("DATASETNAME",4000,1) AS "DATASETNAME",
        DBMS_LOB.SUBSTR("DATASETDESCRIPTION",4000,1) AS "DATASETDESCRIPTION",
        DBMS_LOB.SUBSTR("DATASETVALUE",4000,1) AS "DATASETVALUE",
        DBMS_LOB.SUBSTR("SKIP",4000,1) AS "SKIP",
        "RUN_ORDER"
        FROM (
            SELECT 
                (SELECT  concatenate_application(t.TEST_SUITE_ID) FROM DUAL) AS "Application",
                t.TEST_SUITE_NAME AS "test_suite_name",
                tc.TEST_CASE_NAME as "test_case_name",
                tc.TEST_STEP_DESCRIPTION AS "test_step_description",
                tk.KEY_WORD_NAME as "key_word_name",
                tobn.OBJECT_HAPPY_NAME AS "object_happy_name",
                ts.COLUMN_ROW_SETTING as "parameter",
                ts."COMMENT",
                ( SELECT concatenate_datasetnames(tc.TEST_CASE_ID) DATASETNAME FROM DUAL) "DATASETNAME",
                (SELECT concatenate_datasetdescription(tc.TEST_CASE_ID) FROM DUAL) "DATASETDESCRIPTION",
                (SELECT concatenate_teststepvalue(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "DATASETVALUE",
                (SELECT concatenate_teststepskip(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "SKIP",
                ts."RUN_ORDER"
                FROM T_TEST_SUITE t
            INNER JOIN REL_APP_TESTSUITE relta ON relta.TEST_SUITE_ID = t.TEST_SUITE_ID
            INNER JOIN T_REGISTERED_APPS tapp ON tapp.APPLICATION_ID = relta.APPLICATION_ID
            INNER JOIN REL_TEST_CASE_TEST_SUITE reltcts ON reltcts.TEST_SUITE_ID = t.TEST_SUITE_ID
            INNER JOIN REL_APP_TESTCASE reapp ON reapp.test_case_id = reltcts.TEST_CASE_ID
                and reapp.application_id= relta.application_id AND tapp.APPLICATION_ID = reapp.application_id          
            INNER JOIN T_TEST_CASE_SUMMARY tc ON tc.TEST_CASE_ID = reltcts.TEST_CASE_ID
            LEFT JOIN T_TEST_STEPS ts ON ts.TEST_CASE_ID = tc.TEST_CASE_ID
            LEFT JOIN T_KEYWORD tk ON tk.KEY_WORD_ID = ts.KEY_WORD_ID
            LEFT JOIN T_OBJECT_NAMEINFO tobn ON tobn.OBJECT_NAME_ID = ts.OBJECT_NAME_ID
            WHERE tc.TEST_CASE_ID = TESTCASEID
           ) 
        ORDER BY 3 DESC,13;    
END;
/

create or replace PROCEDURE "USP_FEEDPROCESS" (
FEEDPROCESSID   in number:= 0,
OPERATION in varchar2,
CREATEDBY in varchar2:=null,
FEEDPROCESSSTATUS in varchar2:=null,
ID out number
)
is
begin

-- For Create New FEEDPROCESSID 
if OPERATION ='INSERT' then
		DECLARE
		   CREATEDON1 timestamp;

		BEGIN
		   SELECT sysdate into CREATEDON1 from dual;
		   select  TBLFEEDPROCESS_SEQ.nextval into ID from dual;

		  insert into TBLFEEDPROCESS
			(FEEDPROCESSID, FEEDPROCESSSTATUS, FEEDRUNON, CREATEDBY, CREATEDON)
		  values
			(ID, FEEDPROCESSSTATUS, CREATEDON1, CREATEDBY, CREATEDON1);
		END;

end if;

if OPERATION ='UPDATE' then

		BEGIN


		   update TBLFEEDPROCESS set TBLFEEDPROCESS.FEEDPROCESSSTATUS=USP_FEEDPROCESS.FEEDPROCESSSTATUS where TBLFEEDPROCESS.FEEDPROCESSID=USP_FEEDPROCESS.FEEDPROCESSID;
		  ID:=0;

		END;

end if;


-- for reset sequence use below 
--BEGIN 
  --reset_sequence('TBLFEEDPROCESS_SEQ'); 
--END;
--
  --
end USP_FEEDPROCESS;
/

create or replace PROCEDURE "USP_FEEDPROCESSDETAILS" (
FEEDPROCESSDETAILID   in number:= 0,
FEEDPROCESSID   in number:= 0,
OPERATION in varchar2,
FILENAME in varchar2,
CREATEDBY in varchar2:=null,
FEEDPROCESSSTATUS in varchar2:=null,
FILETYPE in varchar2:=null,
ID out number
)
is
begin

-- For Create New FEEDPROCESSID 
if OPERATION ='INSERT' then
			DECLARE
			   CREATEDON1 timestamp;

			BEGIN
			   SELECT sysdate into CREATEDON1 from dual;
			   select  TBLFEEDPROCESSDETAILS_SEQ.nextval into ID from dual;

			  insert into TBLFEEDPROCESSDETAILS
				(FEEDPROCESSDETAILID,FEEDPROCESSID,FILENAME,FEEDPROCESSSTATUS,CREATEDBY,CREATEDON,FILETYPE)
			  values
				(ID,FEEDPROCESSID,FILENAME, FEEDPROCESSSTATUS,  CREATEDBY, CREATEDON1,FILETYPE);
			END;

end if;
----------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------

if OPERATION ='UPDATE' then

			 UPDATE TBLFEEDPROCESSDETAILS set TBLFEEDPROCESSDETAILS.FEEDPROCESSSTATUS=USP_FEEDPROCESSDETAILS.FEEDPROCESSSTATUS where TBLFEEDPROCESSDETAILS.FEEDPROCESSDETAILID=USP_FEEDPROCESSDETAILS.FEEDPROCESSDETAILID;
			 COMMIT;
			ID:=0;

end if;


-- for reset sequence use below 
--BEGIN 
  --reset_sequence('TBLFEEDPROCESS_SEQ'); 
--END;
--
  --
end USP_FEEDPROCESSDETAILS;
/

create or replace PROCEDURE USP_FEEDPROCESSMAPPING_Mode_B(
    FEEDPROCESSID1 IN NUMBER,
   ISOVERWRITE IN NUMBER,
    RESULT OUT CLOB
)
IS 
BEGIN
		DECLARE


			CURSOR FORTBLFEEDPROCESS IS 
		    SELECT  FEEDPROCESSDETAILID,FEEDPROCESSID,FILENAME,FEEDPROCESSSTATUS,CREATEDBY,CREATEDON,FILETYPE FROM TBLFEEDPROCESSDETAILS WHERE FEEDPROCESSID = FEEDPROCESSID1;
BEGIN
			--OPEN FORTBLFEEDPROCESS;

			FOR I_INDEX IN FORTBLFEEDPROCESS
			LOOP

						---------------------------- START - CODE FOR IMPORT OBJECT FILE ---------------------------------
						--------------------------------------------------------------------------------------------------
						IF I_INDEX.FILETYPE = 'OBJECT' THEN

							DECLARE

							GETTYPEID NUMBER:=0; -- USE IN IMPORT OBJECT
							NEWAPPLICATION_ID NUMBER:=0;
							FULLDATA CLOB;
							GETOBJECTHAPPYNAME VARCHAR2(500):=NULL;
							GETOBJECTNAMEID NUMBER;
							GETOBJECTPARENT VARCHAR2(500):=NULL;
              PARENTOBJECTEXISTS NUMERIC:=0;
							GETOBJECTPARENTID NUMBER;
              CURRENTOBJECTEXISTS NUMERIC:=0;
							CURRENTDATE TIMESTAMP;

							CURSOR FORTBLSTGGUIOBJECT IS
							SELECT OBJECTNAME,TYPE,QUICKACCESS,PARENT,OBJECTCOMMENT,ENUMTYPE,SQL,APPLICATIONNAME,FEEDPROCESSDETAILID FROM TBLSTGGUIOBJECT WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

							BEGIN

									FOR OBJ_INDEX IN FORTBLSTGGUIOBJECT
									LOOP

									   SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

										 FULLDATA := (OBJ_INDEX.OBJECTNAME || '####' || OBJ_INDEX.TYPE || '####' || OBJ_INDEX.QUICKACCESS || '####' || OBJ_INDEX.PARENT || '####' || OBJ_INDEX.OBJECTCOMMENT || '####' || OBJ_INDEX.ENUMTYPE || '####' || OBJ_INDEX."SQL" || '####' || OBJ_INDEX.APPLICATIONNAME || '####' || OBJ_INDEX.FEEDPROCESSDETAILID); 

										 -- CHECK APPLICATION ID IS EXSITS OR NOT
										 SELECT APPLICATION_ID INTO NEWAPPLICATION_ID FROM T_REGISTERED_APPS WHERE UPPER(APP_SHORT_NAME)=UPPER(OBJ_INDEX.APPLICATIONNAME) ; -- TO GET THE NEXT VALUE

                    --dbms_output.put_line('Application ID: ');
                    --dbms_output.put(NEWAPPLICATION_ID);
										 IF NEWAPPLICATION_ID != 0 THEN

												BEGIN
													--- CHECK TYPE_ID IS IN TABLE T_GUI_COMPONENT_TYPE_DIC IF NOT FOUND THEN STOP

													SELECT TYPE_ID  INTO GETTYPEID FROM T_GUI_COMPONENT_TYPE_DIC WHERE UPPER(TYPE_NAME) = UPPER(OBJ_INDEX.TYPE);
													IF  GETTYPEID != 0 THEN
														BEGIN

																--- CHECK PARENT IN TABLE : T_OBJECT_NAMEINFO IF NOT THEN CREATE AND GET THAT ID AND NAME VALUE PAIR
                                --dbms_output.put_line(OBJ_INDEX.PARENT);
																SELECT COUNT(1) INTO PARENTOBJECTEXISTS FROM T_OBJECT_NAMEINFO WHERE OBJECT_HAPPY_NAME=OBJ_INDEX.PARENT AND ROWNUM = 1;

																IF PARENTOBJECTEXISTS = 0 THEN
																	BEGIN





																			INSERT INTO "T_OBJECT_NAMEINFO" (OBJECT_NAME_ID,OBJECT_HAPPY_NAME,PEGWINDOW_MARK,OBJNAME_DESCRIPTION)
																			VALUES(SEQ_MARS_OBJECT_ID.nextval,OBJ_INDEX.OBJECTNAME,0,NULL);

                                                                            SELECT MAX(OBJECT_NAME_ID)  INTO GETOBJECTPARENTID FROM T_OBJECT_NAMEINFO;

                                                                            SELECT OBJECT_HAPPY_NAME  INTO GETOBJECTPARENT FROM T_OBJECT_NAMEINFO WHERE T_OBJECT_NAMEINFO.OBJECT_NAME_ID=GETOBJECTPARENTID;
                                      --dbms_output.put_line('New Object--');
                                      --dbms_output.put(GETOBJECTPARENTID);
                                      --SYS.DBMS_OUTPUT.PUT('--' || GETOBJECTPARENT);
																	END;
                                ELSE BEGIN
                                  SELECT OBJECT_NAME_ID  INTO GETOBJECTPARENTID FROM T_OBJECT_NAMEINFO WHERE OBJECT_HAPPY_NAME=OBJ_INDEX.PARENT AND ROWNUM = 1;
                                  SELECT OBJECT_HAPPY_NAME  INTO GETOBJECTPARENT FROM T_OBJECT_NAMEINFO WHERE T_OBJECT_NAMEINFO.OBJECT_NAME_ID=GETOBJECTPARENTID;
                                  --dbms_output.put_line('Existing Object--');
                                  --dbms_output.put(GETOBJECTPARENTID);
                                  --SYS.DBMS_OUTPUT.PUT('--' || GETOBJECTPARENT);
                                END;
																END IF;


																--- CHECK OBJECT_HAPPY_NAME IN TABLE : T_OBJECT_NAMEINFO IF NOT THEN CREATE AND GET THAT ID AND NAME VALUE PAIR

																SELECT COUNT(1) INTO CURRENTOBJECTEXISTS 
                                FROM T_OBJECT_NAMEINFO 
                                --INNER JOIN T_REGISTED_OBJECT ON T_REGISTED_OBJECT.OBJECT_NAME_ID = T_OBJECT_NAMEINFO.OBJECT_NAME_ID
                                  --AND T_REGISTED_OBJECT.OBJECT_TYPE = OBJ_INDEX.PARENT
                                WHERE T_OBJECT_NAMEINFO.OBJECT_HAPPY_NAME=OBJ_INDEX.OBJECTNAME
                                  --AND T_REGISTED_OBJECT.APPLICATION_ID = NEWAPPLICATION_ID
                                  ;

																IF CURRENTOBJECTEXISTS = 0 THEN
																	BEGIN


																			INSERT INTO "T_OBJECT_NAMEINFO" (OBJECT_NAME_ID,OBJECT_HAPPY_NAME,PEGWINDOW_MARK,OBJNAME_DESCRIPTION)
																			VALUES(SEQ_MARS_OBJECT_ID.nextval,OBJ_INDEX.OBJECTNAME,0,NULL);
                                                                            SELECT MAX(OBJECT_NAME_ID)  INTO GETOBJECTNAMEID FROM T_OBJECT_NAMEINFO;
																	END;
																ELSE
																	BEGIN
																		SELECT T_OBJECT_NAMEINFO.OBJECT_NAME_ID INTO GETOBJECTNAMEID 
                                    FROM T_OBJECT_NAMEINFO 
                                    --INNER JOIN T_REGISTED_OBJECT ON T_REGISTED_OBJECT.OBJECT_NAME_ID = T_OBJECT_NAMEINFO.OBJECT_NAME_ID
                                      --AND T_REGISTED_OBJECT.OBJECT_TYPE = OBJ_INDEX.PARENT
                                    WHERE T_OBJECT_NAMEINFO.OBJECT_HAPPY_NAME=OBJ_INDEX.OBJECTNAME
                                      --AND T_REGISTED_OBJECT.APPLICATION_ID = NEWAPPLICATION_ID 
                                      AND ROWNUM = 1
                                      ;


																	END;

																END IF;


																------ IMPORT IN TABLE : T_REGISTED_OBJECT

																IF GETTYPEID != 0 THEN
																	DECLARE
																		ID NUMBER;
																		SUCCESS VARCHAR2(500);
																		AUTO_LOGREPORT NUMBER:=0;
																	BEGIN

                                          UPDATE T_REGISTED_OBJECT
                                          SET QUICK_ACCESS = OBJ_INDEX.QUICKACCESS
                                          WHERE OBJECT_HAPPY_NAME = OBJ_INDEX.OBJECTNAME
                                            AND APPLICATION_ID = NEWAPPLICATION_ID
                                            AND TYPE_ID = GETTYPEID
                                            AND OBJECT_TYPE = GETOBJECTPARENT;



																					INSERT INTO "T_REGISTED_OBJECT"(
																					OBJECT_ID,
																					OBJECT_HAPPY_NAME,
																					APPLICATION_ID,
																					TYPE_ID,
																					QUICK_ACCESS,
																					OBJECT_TYPE,
																					"COMMENT",
																					ENUM_TYPE,
																					OBJECT_NAME_ID,
																					IS_CHECKERROR_OBJ,
																					OBJ_DATA_SRC
																					)
																					SELECT 
                                            SEQ_MARS_OBJECT_ID.nextval,
                                            OBJ_INDEX.OBJECTNAME,
                                            NEWAPPLICATION_ID,
                                            GETTYPEID,
                                            OBJ_INDEX.QUICKACCESS,
                                            GETOBJECTPARENT,
                                            OBJ_INDEX.OBJECTCOMMENT,
                                            OBJ_INDEX.ENUMTYPE,
                                            GETOBJECTNAMEID,
                                            NULL,
                                            NULL 
                                          FROM DUAL
                                          WHERE NOT EXISTS (
                                            SELECT 1
                                            FROM T_REGISTED_OBJECT x
                                            INNER JOIN T_OBJECT_NAMEINFO y ON y.OBJECT_NAME_ID = x.OBJECT_NAME_ID
                                            WHERE y.OBJECT_HAPPY_NAME = OBJ_INDEX.OBJECTNAME
                                              AND x.APPLICATION_ID = NEWAPPLICATION_ID
                                              AND x.TYPE_ID = GETTYPEID
                                              AND x.OBJECT_TYPE = GETOBJECTPARENT
                                          );
                                        SELECT MAX(OBJECT_ID)  INTO ID FROM T_REGISTED_OBJECT;
																					 -- INSERT ENTRY IN LOG TABLE
																					 SELECT MESSAGE INTO SUCCESS FROM TBLMESSAGE WHERE ID=4;
																					 SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

																					 INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON, feedprocessdetailid)
																					 VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,SUCCESS,FULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
																					 IF RESULT IS NULL THEN
																							RESULT:= 'SUCCESS';
																					 ELSE
																							RESULT:= RESULT || '####' || 'SUCCESS';
																					 END IF;
																	END;
																END IF;

														END;
													ELSE
														DECLARE
															ERROR VARCHAR2(500);
															AUTO_LOGREPORT NUMBER:=0;
														BEGIN
															 SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=1;
															 SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

															INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON, feedprocessdetailid)
															VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,FULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
																IF RESULT IS NULL THEN
																	RESULT:= 'ERROR';
																ELSE
																	RESULT:= RESULT || '####' || 'ERROR';
																END IF;
														END;

													END IF;
												END;
										ELSE
												DECLARE
														ERROR VARCHAR2(500);
														AUTO_LOGREPORT NUMBER:=0;
												BEGIN
														SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=2;

														SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

														INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON, feedprocessdetailid)
														VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,FULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
														IF RESULT IS NULL THEN
																	RESULT:= 'ERROR';
																ELSE
																	RESULT:= RESULT || '####' || 'ERROR';
														END IF;
												END;
									   END IF;

								END LOOP;
								--CLOSE FORTBLSTGGUIOBJECT;
							END;	

						END IF;			
------------------------------------------------------ END - CODE FOR IMPORT OBJECT FILE ---------------------------------
---------------------------------------------------------------------------------------------------------------			

----------------------------------------------------- START - CODE FOR IMPORT COMPAREPARAM ---------------------------------
----------------------------------------------------------------------------------------------------------------------------
						IF I_INDEX.FILETYPE = 'COMPAREPARAM' THEN

							BEGIN
								DECLARE

								COMPAREPARAMFULLDATA CLOB;
								GETDATASOURCENAME VARCHAR2(500):=NULL;
								GETDATASOURCETYPEID NUMBER:=0;
								ERROR VARCHAR2(500);
								CURRENTDATE TIMESTAMP;

								CURSOR FORTBLSTGCOMPAREPARAM IS
								SELECT DATASOURCENAME,DATASOURCETYPE,DETAILS FROM TBLSTGCOMPAREPARAM WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

								BEGIN

									FOR COMPARE_INDEX IN FORTBLSTGCOMPAREPARAM
									LOOP

										SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

										COMPAREPARAMFULLDATA := (COMPARE_INDEX.DATASOURCENAME || '####' || COMPARE_INDEX.DATASOURCETYPE  || '####' || COMPARE_INDEX.DETAILS); 
										-- 1 -- COMPARE -- 2 -- CONNECTION  -- 3 -- QUERY -- 4 -- PROFILE

										IF COMPARE_INDEX.DATASOURCETYPE  IN (1,2,3,4) THEN
											DECLARE
												AUTO_LOGREPORT NUMBER:=0;
											BEGIN
														-- CHECK DATASOURCENAME FROM EXCEL IS AVLIABLE THEN GET DATASOURCETYPEID FROM TABLE IF NOT THEN CREATE NEW AND GET THAT

														SELECT COUNT(1) INTO GETDATASOURCETYPEID FROM T_DATA_SOURCE 
                            WHERE T_DATA_SOURCE.DATA_SOURCE_NAME=COMPARE_INDEX.DATASOURCENAME AND ROWNUM = 1;
                            --dbms_output.put_line(GETDATASOURCETYPEID);
                          	IF GETDATASOURCETYPEID=0 THEN
															BEGIN
																-- CREATE NEW ENTRY

																   INSERT INTO T_DATA_SOURCE(DATA_SOURCE_ID,DATA_SOURCE_NAME,DATA_SOURCE_TYPE,DETAILS,DB_CONNECTION,DB_TYPE,TEST_CONNECTION)
																   VALUES(T_TEST_STEPS_SEQ.nextval,COMPARE_INDEX.DATASOURCENAME,COMPARE_INDEX.DATASOURCETYPE, COMPARE_INDEX.DETAILS,NULL,NULL,NULL);
                                                                   SELECT MAX(DATA_SOURCE_ID) INTO GETDATASOURCETYPEID FROM T_DATA_SOURCE;
															END;
														ELSE
															BEGIN
																-- UPDATE ENTRY
                                IF COMPARE_INDEX.DATASOURCENAME = 'SanjayTest'
                                THEN 
                                BEGIN
                                  dbms_output.put_line(COMPARE_INDEX.DATASOURCENAME || ' -- ' || COMPARE_INDEX.DETAILS);
                                END;
                                END IF;
                                  SELECT DATA_SOURCE_ID INTO GETDATASOURCETYPEID FROM T_DATA_SOURCE 
                                  WHERE T_DATA_SOURCE.DATA_SOURCE_NAME=COMPARE_INDEX.DATASOURCENAME AND ROWNUM = 1;

																   UPDATE T_DATA_SOURCE SET 
																   T_DATA_SOURCE.DATA_SOURCE_TYPE=COMPARE_INDEX.DATASOURCETYPE ,
																   T_DATA_SOURCE.DETAILS=COMPARE_INDEX.DETAILS 
																   WHERE  T_DATA_SOURCE.DATA_SOURCE_ID=GETDATASOURCETYPEID; 
															END;
														END IF;												

														---- INSERT LOG  AS SCCESS IN LOGDETAIL TABLE
														SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=4;
														SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

														INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
														VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,COMPAREPARAMFULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
														IF RESULT IS NULL THEN
																	RESULT:= 'SUCCESS';
																ELSE
																	RESULT:= RESULT || '####' || 'SUCCESS';
														END IF;
											END;
										ELSE
											DECLARE
												AUTO_LOGREPORT NUMBER:=0;
											BEGIN
														SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=3;
														SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

														INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
														VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,COMPAREPARAMFULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
														IF RESULT IS NULL THEN
																	RESULT:= 'ERROR';
																ELSE
																	RESULT:= RESULT || '####' || 'ERROR';
														END IF;
											END;
										END IF;
									END LOOP;
									--CLOSE FORTBLSTGCOMPAREPARAM;
								END;

							END;

						END IF;

----------------------------------------------------- END - CODE FOR IMPORT COMPAREPARAM ---------------------------------
--------------------------------------------------------------------------------------------------------------------------

----------------------------------------------------- START - CODE FOR IMPORT TEST CASE ---------------------------------
-------------------------------------------------------------------------------------------------------------------------


						IF I_INDEX.FILETYPE = 'TESTCASE' THEN
							DECLARE
									VALIDATEMESSAGE VARCHAR2(5000);
									TESTSUITECOUNT NUMBER:=0;
									APPLICATIONID NUMBER:=0;
									COUNT_APPLICATIONID NUMBER:=0;
									CURRENTDATE TIMESTAMP;
									TESTSUITENAME VARCHAR(5000);
					                TESTCASENAME VARCHAR(5000);
							BEGIN

                                    -- Remove duplicate staging rows

                                    DELETE FROM tblstgtestcase
                                    WHERE ID IN (
                                        SELECT DISTINCT ID
                                        from tblstgtestcase t
                                        where t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                        AND EXISTS (
                                            SELECT 1
                                            FROM tblstgtestcase s
                                            WHERE s.ID > t.ID
                                                AND s.feedprocessdetailid = t.feedprocessdetailid
                                                AND (
                                                    --t.TESTSUITENAME = s.TESTSUITENAME AND  t.TESTCASENAME = s.TESTCASENAME  AND t.TESTCASEDESCRIPTION = s.TESTCASEDESCRIPTION AND NVL(t.DATASETMODE, 'N/A') = NVL(s.DATASETMODE, 'N/A') AND  t.KEYWORD = s.KEYWORD AND  NVL(t.OBJECT, 'N/A') = NVL(s.OBJECT, 'N/A') AND  NVL(t.PARAMETER, 'N/A') = NVL(s.PARAMETER, 'N/A') AND NVL(t.COMMENTS, 'N/A') = NVL(s.COMMENTS, 'N/A') AND t.DATASETNAME = s.DATASETNAME AND  t.DATASETVALUE = s.DATASETVALUE AND  t.ROWNUMBER = s.ROWNUMBER AND  t.FEEDPROCESSDETAILID = s.FEEDPROCESSDETAILID AND  t.TABNAME = s.TABNAME AND  t.APPLICATION = s.APPLICATION AND  t.CREATEDON = s.CREATEDON
                                                    t.TESTSUITENAME = s.TESTSUITENAME AND  t.TESTCASENAME = s.TESTCASENAME  AND t.TESTCASEDESCRIPTION = s.TESTCASEDESCRIPTION AND NVL(t.DATASETMODE, 'N/A') = NVL(s.DATASETMODE, 'N/A') AND  t.KEYWORD = s.KEYWORD AND  NVL(t.OBJECT, 'N/A') = NVL(s.OBJECT, 'N/A') AND  NVL(t.PARAMETER, 'N/A') = NVL(s.PARAMETER, 'N/A') AND NVL(t.COMMENTS, 'N/A') = NVL(s.COMMENTS, 'N/A') AND t.DATASETNAME = s.DATASETNAME AND  NVL(t.DATASETVALUE, 'N/A') = NVL(s.DATASETVALUE, 'N/A') AND  t.FEEDPROCESSDETAILID = s.FEEDPROCESSDETAILID AND  t.TABNAME = s.TABNAME AND  t.APPLICATION = s.APPLICATION AND s.rownumber = t.rownumber
                                                )
                                        )
                                    );    
                                    commit;    

									SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

									SELECT COUNT(*) INTO COUNT_APPLICATIONID FROM T_REGISTERED_APPS,TBLSTGTESTCASE WHERE  
									T_REGISTERED_APPS.APP_SHORT_NAME= TBLSTGTESTCASE.APPLICATION
									AND TBLSTGTESTCASE.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

								IF (COUNT_APPLICATIONID = 0) THEN
									BEGIN
                                        SELECT count(*) INTO COUNT_APPLICATIONID
                                        FROM (
                                            SELECT Application 
                                            FROM (
                                                    SELECT 
                                                        trim(regexp_substr(Application, '[^,]+', 1, level)) Application
                                                    FROM (
                                                        SELECT DISTINCT Application 
                                                        FROM TBLSTGTESTCASE 
                                                        WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                    ) t
                                                    CONNECT BY instr(Application, ',', 1, level - 1) > 0
                                            ) xy

                                            WHERE EXISTS (
                                                SELECT 1
                                                FROM T_REGISTERED_APPS 
                                                WHERE T_REGISTERED_APPS.APP_SHORT_NAME = xy.Application
                                            )
                                        ) xyz;

									end;
									end if;		    

								IF COUNT_APPLICATIONID!=0 THEN
									DECLARE
										AUTO_TEMP1 NUMBER:=0;
									BEGIN

                                    --dbms_output.put_line('COUNT_APPLICATIONID '||COUNT_APPLICATIONID);
											-- Code developed by Shivam -- Start
                                            					SELECT DISTINCT upper(TBLSTGTESTCASE.TESTSUITENAME) INTO TESTSUITENAME
					                                        FROM TBLSTGTESTCASE
                                            					WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID
                                                                AND ROWNUM = 1;



					                                            -- Code for inserting folse Application in the temp table.

					                                            -- Delete data from temporary table
                                            					DELETE FROM TEMPFALSEAPPLICATION;

                                            					-- Insert the false data
                                            INSERT INTO TEMPFALSEAPPLICATION
                                            (
                                                TESTSUITNAME,
                                                TESTCASENAME,
                                                APPLICATION,
                                                Flag
                                            )
                                            SELECT TESTSUITENAME,x.TESTCASENAME,Application,CASE WHEN EXISTS (SELECT 1 FROM T_REGISTERED_APPS WHERE T_REGISTERED_APPS.APP_SHORT_NAME = x.Application) THEN 1 ELSE 0 END
                                            FROM (
                                                SELECT DISTINCT
                                                    trim(regexp_substr(Application, '[^,]+', 1, level)) Application,testcasename
                                                FROM (
                                                        SELECT DISTINCT Application, tblstgtestcase.testcasename
                                                        FROM TBLSTGTESTCASE 
                                                        WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                     ) t
                                                CONNECT BY instr(Application, ',', 1, level - 1) > 0
                                                order by Application
                                            ) x;
                                            -- 
                                            -- Code developed by Shivam -- End

                                            -- GET TOP 1 Application_ID
                                        SELECT NVL(APPLICATION_ID, 0) INTO APPLICATIONID
                                        FROM T_REGISTERED_APPS
                                        WHERE APP_SHORT_NAME IN (
                                          SELECT APPLICATION
                                          FROM TEMPFALSEAPPLICATION
                                          WHERE ROWNUM = 1
                                        );

                                            delete from TEMP1;
											SELECT  TEMP1_SEQ.NEXTVAL INTO AUTO_TEMP1 FROM DUAL;

											INSERT INTO TEMP1 (TESTCASENAME)
											SELECT TESTSUITENAME FROM TBLSTGTESTCASE  WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID GROUP BY  TESTSUITENAME;

											SELECT COUNT(*) INTO TESTSUITECOUNT FROM TEMP1;
--dbms_output.put_line('TESTSUITECOUNT '||TESTSUITECOUNT);

							IF TESTSUITECOUNT = 1 THEN -- IF - 2 -START
									DECLARE
											AUTO_TEMP1 NUMBER:=0;
											AUTO_TEMPDWTESTCASE NUMBER:=0;  -- UNDER OBSERVATION
											AUTO_TEMPSTGTESTCASE NUMBER:=0; -- UNDER OBSERVATION
											CURRENTDATE TIMESTAMP;

									BEGIN

											--- LOGIC FOR ORDER MATCH -

											SELECT SYSDATE INTO CURRENTDATE FROM DUAL;
											DELETE FROM TEMP1;


											SELECT  TEMPSTGTESTCASE_SEQ.NEXTVAL INTO AUTO_TEMPSTGTESTCASE FROM DUAL;

											INSERT INTO TEMPSTGTESTCASE(TESTCASENAME,DATASETNAME,KEYWORDNAME,OBJECTNAME,ORDERNUMBER) 
											SELECT DISTINCT TESTCASENAME,DATASETNAME,KEYWORD,OBJECT,ROWNUMBER FROM TBLSTGTESTCASE WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;


											SELECT  TEMPDWTESTCASE_SEQ.NEXTVAL INTO AUTO_TEMPDWTESTCASE FROM DUAL;

                                            INSERT INTO TEMPDWTESTCASE(TESTCASENAME,DATASETNAME,KEYWORDNAME,OBJECTNAME,ORDERNUMBER) 
                                            SELECT DISTINCT
                                                T_TEST_CASE_SUMMARY.TEST_CASE_NAME,
                                                DATASETNAME,T_KEYWORD.KEY_WORD_NAME,TBLSTGTESTCASE.OBJECT,TBLSTGTESTCASE.ROWNUMBER 
                                            FROM TBLSTGTESTCASE
                                            INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                            INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME)  =UPPER(TBLSTGTESTCASE.KEYWORD)
                                            WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

											/*
                                            INSERT INTO TEMPDWTESTCASE(TESTCASENAME,DATASETNAME,KEYWORDNAME,OBJECTNAME,ORDERNUMBER) 
											SELECT 
											T_TEST_CASE_SUMMARY.TEST_CASE_NAME,
											DATASETNAME,T_KEYWORD.KEY_WORD_NAME,T_REGISTED_OBJECT.OBJECT_HAPPY_NAME,TBLSTGTESTCASE.ROWNUMBER 
                                            FROM T_TEST_CASE_SUMMARY ,T_KEYWORD,T_REGISTED_OBJECT,TBLSTGTESTCASE
											WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(TBLSTGTESTCASE.TESTCASENAME)
											AND UPPER(T_KEYWORD.KEY_WORD_NAME)  =UPPER(TBLSTGTESTCASE.KEYWORD)
											--AND UPPER(T_REGISTED_OBJECT.OBJECT_HAPPY_NAME)=UPPER(TBLSTGTESTCASE.OBJECT)
											AND TBLSTGTESTCASE.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID GROUP BY T_TEST_CASE_SUMMARY.TEST_CASE_NAME,
											DATASETNAME,T_KEYWORD.KEY_WORD_NAME,T_REGISTED_OBJECT.OBJECT_HAPPY_NAME,TBLSTGTESTCASE.ROWNUMBER
											ORDER BY TBLSTGTESTCASE.ROWNUMBER;
                                            */



											SELECT  TEMP1_SEQ.NEXTVAL INTO AUTO_TEMP1 FROM DUAL;

											INSERT INTO TEMP1 (TESTCASENAME,DATASETNAME)
											SELECT DISTINCT TEMPSTGTESTCASE.TESTCASENAME,TEMPSTGTESTCASE.DATASETNAME
											FROM TEMPSTGTESTCASE
											WHERE NOT EXISTS(SELECT TEMPDWTESTCASE.TESTCASENAME,TEMPDWTESTCASE.DATASETNAME 
											FROM TEMPDWTESTCASE 
											WHERE UPPER(TEMPDWTESTCASE.TESTCASENAME) =UPPER(TEMPSTGTESTCASE.TESTCASENAME)
											AND UPPER(TEMPDWTESTCASE.DATASETNAME)=UPPER(TEMPSTGTESTCASE.DATASETNAME)
											AND UPPER(TEMPDWTESTCASE.KEYWORDNAME)=UPPER(TEMPSTGTESTCASE.KEYWORDNAME)
											--AND UPPER(TEMPDWTESTCASE.OBJECTNAME)=UPPER(TEMPSTGTESTCASE.OBJECTNAME)
											AND TEMPDWTESTCASE.ORDERNUMBER = TEMPSTGTESTCASE.ORDERNUMBER
											);



----------------------------------------------------------------------------------------------------------------------------------------
                            --dbms_output.put_line('Overwrite Value: ' || ISOVERWRITE);
                    /*	IF ISOVERWRITE != 0 THEN  -- 0 MEANS FALSE ELSE TRUE LIKE 1 -- IF - 3 -START

----------------------------------------------------------- DELETE FROM DATAWARE HOUSE CODE - START --------------------------------------------------------------------					
                                        -- Performance code by Shivam Parikh - Start
                                        DECLARE
                                            ERROR varchar2(5000);
                                            COUNTOFRECORDS INT;
                                        BEGIN  

                                            -- Addd Log of all delete records from warehouse - Start
                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=20;

                                                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,LOGDETAIL,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                                                FROM (    
                                                    SELECT DISTINCT 'TESTCASE NAME :' || tblstgtestcase.TESTCASENAME  || ' : ' || tblstgtestcase.DATASETNAME AS LOGDETAIL
                                                    FROM tblstgtestcase
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                ) xyz;

                                                -- Add log for Success Found - Start
                                                SELECT count(1) INTO COUNTOFRECORDS
                                                FROM tblstgtestcase
                                                WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                    AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                    AND EXISTS (
                                                        SELECT 1
                                                        FROM T_TEST_CASE_SUMMARY 
                                                        WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    );
                                                IF (COUNTOFRECORDS > 0) 
                                                THEN    
                                                    BEGIN
                                                        IF RESULT IS NULL THEN
                                                                    RESULT:= 'SUCCESS';
                                                                ELSE
                                                                    RESULT:= RESULT || '####' || 'SUCCESS';
                                                        END IF;
                                                    END;
                                                END IF;
                                                -- Add log for Success Found - End
                                            -- Addd Log of all delete records from warehouse - Start


                                            -- Add log for Test Case Not Found - Start
                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=19;

                                                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,LOGDETAIL,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                                                FROM (    
                                                    SELECT DISTINCT tblstgtestcase.TESTCASENAME AS LOGDETAIL
                                                    FROM tblstgtestcase
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM T_TEST_CASE_SUMMARY 
                                                            WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                        )
                                                ) xyz;

                                                -- Append RESULT for Error

                                                SELECT COUNT(1) INTO COUNTOFRECORDS
                                                FROM tblstgtestcase
                                                WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                    AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM T_TEST_CASE_SUMMARY 
                                                        WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    );
                                                IF (COUNTOFRECORDS > 0)
                                                THEN
                                                    BEGIN
                                                        IF RESULT IS NULL THEN
                                                                    RESULT:= 'ERROR';
                                                                ELSE
                                                                    RESULT:= RESULT || '####' || 'ERROR';
                                                        END IF;
                                                    END;
                                                END IF;
                                                -- Append RESULT for Error
                                            -- Add log for Test Case Not Found - End

                                            -- Data Set Delete code -- Start

                                                dbms_output.put_line('delete started');
                                                DELETE FROM REL_TC_DATA_SUMMARY 
                                                WHERE REL_TC_DATA_SUMMARY.DATA_SUMMARY_ID IN (
                                                    SELECT DISTINCT REL_TC_DATA_SUMMARY.DATA_SUMMARY_ID
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    INNER JOIN REL_TC_DATA_SUMMARY ON REL_TC_DATA_SUMMARY.TEST_CASE_ID = T_TEST_CASE_SUMMARY.TEST_CASE_ID
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Data Set Delete code -- End

                                            -- Data Set Value delete code - Start
                                                DELETE FROM TEST_DATA_SETTING 
                                                WHERE TEST_DATA_SETTING.STEPS_ID IN 
                                                (
                                                    SELECT T_TEST_STEPS.STEPS_ID 
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.TEST_CASE_ID = T_TEST_CASE_SUMMARY.TEST_CASE_ID
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Data Set Value delete code - Start

                                            -- Delete from T_TEST_STEPS - Start
                                                DELETE FROM T_TEST_STEPS 
                                                WHERE T_TEST_STEPS.TEST_CASE_ID IN (
                                                    SELECT T_TEST_CASE_SUMMARY.TEST_CASE_ID
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Delete from T_TEST_STEPS - Start

                                            -- Delete from REL_APP_TESTCASE - Start
                                                DELETE FROM REL_APP_TESTCASE 
                                                WHERE REL_APP_TESTCASE.TEST_CASE_ID IN (
                                                    SELECT T_TEST_CASE_SUMMARY.TEST_CASE_ID
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Delete from REL_APP_TESTCASE - End

                                            -- Delete from REL_TEST_CASE_TEST_SUITE - Start

                                            -- Delete from REL_TEST_CASE_TEST_SUITE - Start
                                                DELETE FROM REL_TEST_CASE_TEST_SUITE
                                                WHERE rel_test_case_test_suite.relationship_id IN (
                                                    SELECT DISTINCT rel_test_case_test_suite.relationship_id
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    INNER JOIN t_test_suite ON UPPER(t_test_suite.test_suite_name) = UPPER(tblstgtestcase.testsuitename)
                                                    INNER JOIN rel_test_case_test_suite ON rel_test_case_test_suite.test_case_id = T_TEST_CASE_SUMMARY.test_case_id
                                                        AND rel_test_case_test_suite.test_suite_id = t_test_suite.test_suite_id
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Delete from REL_TEST_CASE_TEST_SUITE - End    

                                            -- Delete Test Case - Start
                                                DELETE FROM T_TEST_CASE_SUMMARY
                                                WHERE T_TEST_CASE_SUMMARY.TEST_CASE_ID IN (
                                                    SELECT DISTINCT T_TEST_CASE_SUMMARY.TEST_CASE_ID
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Delete Test Case - End

                                        END;   
                                        dbms_output.put_line('delete ended');
                                        -- Performance code by Shivam Parikh - Start

---------------------------------------------------------------------------------------------------------- DELETE FROM DATAWARE HOUSE CODE - END --------------------------------------------------------------------


---------------------------------------------------------------------------------------------------------- LOGIC FOR OVERWRITE - START ----------------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------------- LOGIC FOR OVERWRITE - END ----------------------------------------------------------------------------------------------------------
                            END IF;  
                            */
---------------------------------------------------------------------------------------------------------- LOGIC FOR NOT OVERWRITE - START ----------------------------------------------------------------------------------------------------------

								--BEGIN -- LOGIC FOR NOT OVERWRITE - START

								  --- CODE FOR IF OVERWITE CONDITION IS FALSE

                            -- Performance Code -- Shivam  - Start
                                DECLARE 
                                    MAXRow INT;
                                    OBJECT_ID INT;
                                    OBJECT_ID_Set INT;
                                    AUTO_LOGREPORT INT;
                                    ERROR VARCHAR2(500);
                                    MESSAGE VARCHAR2(500);
                                    CURRENTTIME TIMESTAMP;
                                    COUNT_T_TEST_STEPS INT:=0;
                                    COUNT_T_SHARED_OBJECT_POOL INT:=0;
                                    COUNT_TEST_DATA_SETTING INT:=0;
                                    COUNT_DATA_SET_DESCRIPTION INT:=0;
                                BEGIN    
                                    --dbms_output.put_line('Log Insert Started');
                                    SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                    --dbms_output.put_line(CURRENTTIME);
                                -- Insert All Log Reports - Start  
                                    -- Insert log for the application not found for the tab.
                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=18;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,STATUS,LOGDETAILS, (LOGREPORT_SEQ.NEXTVAL),(SELECT SYSDATE FROM DUAL) AS CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT 'DATASET : '|| tblstgtestcase.DATASETNAME || ERROR || '-' || tblstgtestcase.TABNAME AS STATUS, tblstgtestcase.TESTSUITENAME || '####' ||tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.TESTCASEDESCRIPTION  || '####' || tblstgtestcase.DATASETMODE  || '####' || tblstgtestcase.KEYWORD  || '####' || tblstgtestcase.OBJECT  || '####' || tblstgtestcase.PARAMETER  || '####' || tblstgtestcase.COMMENTS  || '####' || tblstgtestcase.DATASETNAME  || '####' || tblstgtestcase.DATASETVALUE  || '####' || tblstgtestcase.ROWNUMBER  || '####' || tblstgtestcase.FEEDPROCESSDETAILID || '####' || tblstgtestcase.TABNAME LOGDETAILS
                                            FROM tblstgtestcase
                                            INNER JOIN TEMPFALSEAPPLICATION ON TEMPFALSEAPPLICATION.TESTSUITNAME = tblstgtestcase.testsuitename
                                                AND TEMPFALSEAPPLICATION.TESTCASENAME = tblstgtestcase.testcasename
                                                AND tempfalseapplication.flag = 0
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                        ) x;    

                                        IF RESULT IS NULL THEN
                                                    RESULT:= 'ERROR';
                                                ELSE
                                                    RESULT:= RESULT || '####' || 'ERROR';
                                        END IF;
                                    -- Insert log for the application not found for the tab.

                                    -- Inser log for succcess new insert records - Start

                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=4;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,STATUS,LOGDETAILS,LOGREPORT_SEQ.NEXTVAL,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT 'DATASET : '|| tblstgtestcase.DATASETNAME || ERROR || '-' || tblstgtestcase.TABNAME AS STATUS,tblstgtestcase.TESTSUITENAME || '####' || tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.TESTCASEDESCRIPTION  || '####' || tblstgtestcase.DATASETMODE  || '####' || tblstgtestcase.KEYWORD  || '####' || tblstgtestcase.OBJECT  || '####' || tblstgtestcase.PARAMETER  || '####' || tblstgtestcase.COMMENTS  || '####' || tblstgtestcase.DATASETNAME  || '####' || tblstgtestcase.DATASETVALUE  || '####' || tblstgtestcase.ROWNUMBER  || '####' || tblstgtestcase.FEEDPROCESSDETAILID || '####' || tblstgtestcase.TABNAME AS LOGDETAILS
                                            FROM tblstgtestcase
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM tempfalseapplication
                                                    where tempfalseapplication.testcasename = TBLSTGTESTCASE.TESTCASENAME
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                        );        
                                        IF RESULT IS NULL THEN
                                                    RESULT:= 'SUCCESS';
                                                ELSE
                                                    RESULT:= RESULT || '####' || 'SUCCESS';
                                        END IF;
                                    -- Inser log for succcess new insert/Update records - Start


                                    -- Inser log for Keyword not found - Start
                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=1;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,LOGDETAILS,LOGREPORT_SEQ.NEXTVAL,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT tblstgtestcase.TESTSUITENAME || '####' || tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.TESTCASEDESCRIPTION  || '####' || tblstgtestcase.DATASETMODE  || '####' || tblstgtestcase.KEYWORD  || '####' || tblstgtestcase.OBJECT  || '####' || tblstgtestcase.PARAMETER  || '####' || tblstgtestcase.COMMENTS  || '####' || tblstgtestcase.DATASETNAME  || '####' || tblstgtestcase.DATASETVALUE  || '####' || tblstgtestcase.ROWNUMBER  || '####' || tblstgtestcase.FEEDPROCESSDETAILID || '####' || tblstgtestcase.TABNAME LOGDETAILS
                                            FROM tblstgtestcase
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                             and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM t_keyword 
                                                    WHERE UPPER(t_keyword.key_word_name) = UPPER(tblstgtestcase.KEYWORD)

                                                    AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM tempfalseapplication
                                                    where tempfalseapplication.testcasename = TBLSTGTESTCASE.TESTCASENAME
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                        );        
                                        IF RESULT IS NULL THEN
                                                    RESULT:= 'SUCCESS';
                                                ELSE
                                                    RESULT:= RESULT || '####' || 'SUCCESS';
                                        END IF;
                                    -- Inser log for Keyword not found - End

                                    -- Insert log for count of dataset in spreadsheet and count of datasets in warehouse does not match - start

                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=7;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                        --VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,'DATASET : '|| TESTCASE_INDEX.DATASETNAME || ERROR || '-' || TESTCASE_INDEX.TABNAME,TESTCASEFULLDATA,AUTO_LOGREPORT,CURRENTDATE);
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE, STATUS, '' AS LOGDETAILS, (LOGREPORT_SEQ.NEXTVAL), (SELECT SYSDATE FROM DUAL) AS CURRENTDATE ,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT 'DATASET : '|| x.TESTCASENAME || ERROR || '-' || x.DATASETNAME as STATUS
                                            FROM (
                                                SELECT TESTCASENAME,DATASETNAME,COUNT(1) as countofsteps
                                                FROM TBLSTGTESTCASE
                                                WHERE KEYWORD IS NOT NULL 
                                                    AND FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                    AND DATASETMODE IS NULL
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM TEMPFALSEAPPLICATION
                                                        WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                            AND tempfalseapplication.FLAG = 0
                                                            AND ROWNUM=1
                                                    )
                                                GROUP BY TESTCASENAME,DATASETNAME
                                            ) x    
                                            INNER JOIN 
                                            (
                                                SELECT tblstgtestcase.TESTCASENAME, tblstgtestcase.DATASETNAME, COUNT(1) as countofsteps
                                                FROM tblstgtestcase
                                                INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.TESTCASENAME)
                                                INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.DATASETNAME)
                                                INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgtestcase.KEYWORD)
                                                INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.TEST_CASE_ID
                                                    AND T_TEST_STEPS.KEY_WORD_ID = T_KEYWORD.KEY_WORD_ID
                                                    AND T_TEST_STEPS.RUN_ORDER = tblstgtestcase.ROWNUMBER
                                                    AND T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                WHERE tblstgtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                and  tblstgtestcase.KEYWORD IS NOT NULL 
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM TEMPFALSEAPPLICATION
                                                        WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                            AND tempfalseapplication.FLAG = 0
                                                            AND ROWNUM=1
                                                    )
                                                GROUP BY tblstgtestcase.TESTCASENAME, tblstgtestcase.DATASETNAME
                                                ORDER BY 2
                                            ) y
                                            ON x.TESTCASENAME = y.TESTCASENAME
                                                AND x.DATASETNAME = y.DATASETNAME
                                                AND x.countofsteps <> y.countofsteps     
                                        ) xyz;        
                                    -- Insert log for count of dataset in spreadsheet and count of datasets in warehouse does not match - end

                                    -- Insert log for Order mathced but Step ID not found - Start
                                    --=== NEED TO DISCUSS THE BEHAVIOR    
                                    -- Insert log for Order mathced but Step ID not found - End

                                    -- Insert log for records where Order of test case not matched - start

                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=13;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,STATUS,LOGDETAILS,LOGREPORT_SEQ.NEXTVAL, (SELECT SYSDATE FROM DUAL) AS CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT FEEDPROCESSDETAILID,'DATASET : '|| tblstgtestcase.DATASETNAME || ERROR || '-' || tblstgtestcase.TABNAME AS STATUS,tblstgtestcase.TESTSUITENAME || '####' || tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.TESTCASEDESCRIPTION  || '####' || tblstgtestcase.DATASETMODE  || '####' || tblstgtestcase.KEYWORD  || '####' || tblstgtestcase.OBJECT  || '####' || tblstgtestcase.PARAMETER  || '####' || tblstgtestcase.COMMENTS  || '####' || tblstgtestcase.DATASETNAME  || '####' || tblstgtestcase.DATASETVALUE  || '####' || tblstgtestcase.ROWNUMBER  || '####' || tblstgtestcase.FEEDPROCESSDETAILID || '####' || tblstgtestcase.TABNAME AS LOGDETAILS
                                            FROM tblstgtestcase
                                            INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                                AND tblstgtestcase.datasetname IS NOT NULL
                                            INNER JOIN T_KEYWORD ON T_KEYWORD.KEY_WORD_NAME = tblstgtestcase.KEYWORD
                                            CROSS JOIN TABLE(GETTOPOBJECT(tblstgtestcase.object, APPLICATIONID)) t
                                            WHERE tblstgtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.datasetvalue IS NOT NULL
                                                AND tblstgtestcase.datasetmode IS NULL
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM t_test_steps
                                                    WHERE t_test_steps.key_word_id = t_keyword.key_word_id
                                                        AND t_test_steps.object_id = t.OBJECT_ID
                                                        AND t_test_steps.test_case_id = T_TEST_CASE_SUMMARY.test_case_id
                                                        AND t_test_steps.run_order = tblstgtestcase.rownumber
                                                        AND ROWNUM=1
                                                )
                                        );
                                    -- Insert log for records where Order of test case not matched - end

                                    -- Insert log for records where Order of test case not matched - start
                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=8;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,STATUS,LOGDETAILS,LOGREPORT_SEQ.NEXTVAL, (SELECT SYSDATE FROM DUAL) AS CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT FEEDPROCESSDETAILID,'DATASET : '|| tblstgtestcase.DATASETNAME || ERROR || '-' || tblstgtestcase.TABNAME AS STATUS,tblstgtestcase.TESTSUITENAME || '####' || tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.TESTCASEDESCRIPTION  || '####' || tblstgtestcase.DATASETMODE  || '####' || tblstgtestcase.KEYWORD  || '####' || tblstgtestcase.OBJECT  || '####' || tblstgtestcase.PARAMETER  || '####' || tblstgtestcase.COMMENTS  || '####' || tblstgtestcase.DATASETNAME  || '####' || tblstgtestcase.DATASETVALUE  || '####' || tblstgtestcase.ROWNUMBER  || '####' || tblstgtestcase.FEEDPROCESSDETAILID || '####' || tblstgtestcase.TABNAME AS LOGDETAILS
                                            FROM tblstgtestcase
                                            INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                                AND tblstgtestcase.datasetname IS NOT NULL
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                            INNER JOIN T_KEYWORD ON T_KEYWORD.KEY_WORD_NAME = tblstgtestcase.KEYWORD
                                            CROSS JOIN TABLE(GETTOPOBJECT(tblstgtestcase.object, APPLICATIONID)) t
                                            WHERE tblstgtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.datasetvalue IS NOT NULL
                                                AND tblstgtestcase.datasetmode IS NULL
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                                 AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM t_test_steps
                                                    WHERE t_test_steps.key_word_id = t_keyword.key_word_id
                                                        AND t_test_steps.object_id = t.OBJECT_ID
                                                        AND t_test_steps.test_case_id = T_TEST_CASE_SUMMARY.test_case_id
                                                        AND ROWNUM=1
                                                        --AND t_test_steps.run_order = tblstgtestcase.rownumber
                                                )
                                        );
                                    -- Insert log for records where Order of test case not matched - end

                                    -- Insert log for Skip Mode - start
                                        INSERT INTO "LOGREPORT" (FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                                        SELECT tbl.filename,tbl.filetype,'DATASETMODE:-X AND DATASET : ' || tbl.DATASETNAME || msg.MESSAGE AS Status,'' AS LogDetails,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT fd.filename,fd.filetype,stg.datasetname,'' aS LOGDETAILS 
                                            FROM TBLSTGTESTCASE stg
                                            INNER JOIN tblfeedprocessdetails fd ON fd.feedprocessdetailid = stg.feedprocessdetailid
                                            WHERE stg.FEEDPROCESSDETAILID= I_INDEX.FEEDPROCESSDETAILID 
                                                AND stg.DATASETMODE IS NOT NULL  
                                                 and stg.KEYWORD IS NOT NULL 
                                                AND stg.DATASETMODE='X'
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = stg.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                        ) tbl,TBLMESSAGE msg
                                        WHERE msg.ID = 9;
                                    -- Insert log for Skip Mode - end
                                -- Insert All Log Reports - End
                                --dbms_output.put_line('Log Insert Ended');
                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                --dbms_output.put_line(CURRENTTIME);

                                --dbms_output.put_line('All Update Started');
                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                --dbms_output.put_line(CURRENTTIME);
                                -- All update Codes - start

                                -- Update T_TEST_STEPS - Start
                                SELECT COUNT(*) INTO COUNT_T_TEST_STEPS
                                            FROM tblstgtestcase stg
                                            INNER JOIN t_test_case_summary tcs ON UPPER(tcs.test_case_name) = UPPER(stg.testcasename)
                                            INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                            INNER JOIN T_TEST_STEPS tts ON tts.KEY_WORD_ID = tk.KEY_WORD_ID
                                                AND tts.RUN_ORDER = stg.ROWNUMBER
                                                AND tts.TEST_CASE_ID = tcs.test_case_id
                                            CROSS JOIN TABLE(GETTOPOBJECT(stg.object, APPLICATIONID)) t
                                            WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND stg.DATASETMODE IS NULL
                                                 and stg.KEYWORD IS NOT NULL ;
                                IF COUNT_T_TEST_STEPS !=0 THEN

                                     MERGE INTO T_TEST_STEPS TTS1
                                        USING (
                                            SELECT DISTINCT tts.STEPS_ID,t.OBJECT_ID, t.OBJECT_NAME_ID, stg.parameter, stg.COMMENTS
                                            FROM tblstgtestcase stg
                                            INNER JOIN t_test_case_summary tcs ON UPPER(tcs.test_case_name) = UPPER(stg.testcasename)
                                            INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                            INNER JOIN T_TEST_STEPS tts ON tts.KEY_WORD_ID = tk.KEY_WORD_ID
                                                AND tts.RUN_ORDER = stg.ROWNUMBER
                                                AND tts.TEST_CASE_ID = tcs.test_case_id
                                            CROSS JOIN TABLE(GETTOPOBJECT(stg.object, APPLICATIONID)) t
                                            WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND stg.DATASETMODE IS NULL
                                                 and stg.KEYWORD IS NOT NULL 
                                        ) TMP
                                        ON (TTS1.STEPS_ID = TMP.STEPS_ID)
                                        WHEN MATCHED THEN
                                        UPDATE SET
                                            TTS1.OBJECT_ID = TMP.OBJECT_ID,
                                            TTS1.COLUMN_ROW_SETTING = TMP.parameter,
                                            TTS1.OBJECT_NAME_ID = TMP.OBJECT_NAME_ID,
                                            TTS1."COMMENT" = TMP.COMMENTS;
                                    END IF;
                                -- Update T_TEST_STEPS - End

                                -- Update T_SHARED_OBJECT_POOL - Start
                                SELECT COUNT(*) INTO COUNT_T_SHARED_OBJECT_POOL
                                            FROM tblstgtestcase stg
                                            INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                            INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                                AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                                AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                            WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND stg.DATASETMODE IS NULL
                                                and   stg.KEYWORD IS NOT NULL ;
                                IF COUNT_T_SHARED_OBJECT_POOL !=0 THEN

                                        DECLARE
                                          OBJPOOLidu INT:=0;
                                          DATASETVALUEU VARCHAR2(1000):='';
                                          CURSOR TSHAREDOBJ
                                          IS
                                          SELECT DISTINCT tsop.OBJECT_POOL_ID,stg.DATASETVALUE
                                          FROM tblstgtestcase stg
                                          INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                          INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                              AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                              AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                          WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                              AND stg.DATASETMODE IS NULL
                                               and stg.KEYWORD IS NOT NULL;
                                          BEGIN

                                            FOR TSHAREDOBJ_INDEX IN TSHAREDOBJ
                                            LOOP

                                              UPDATE T_SHARED_OBJECT_POOL
                                              SET DATA_VALUE = TSHAREDOBJ_INDEX.DATASETVALUE
                                              WHERE OBJECT_POOL_ID = TSHAREDOBJ_INDEX.OBJECT_POOL_ID;

                                            END LOOP;

                                          END;
                                          /*
                                         MERGE INTO T_SHARED_OBJECT_POOL tsop1
                                            USING (
                                                SELECT DISTINCT tsop.OBJECT_POOL_ID,stg.DATASETVALUE
                                                FROM tblstgtestcase stg
                                                INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                                INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                                    AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                                    AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                                WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                    AND stg.DATASETMODE IS NULL
                                                     and stg.KEYWORD IS NOT NULL 
                                            ) TMP
                                            ON (tsop1.OBJECT_POOL_ID = TMP.OBJECT_POOL_ID)
                                            WHEN MATCHED THEN
                                            UPDATE SET
                                                tsop1.DATA_VALUE = TMP.DATASETVALUE;
                                         */
                                        END IF;
                                -- Update T_SHARED_OBJECT_POOL - Start

                                -- Update TEST_DATA_SETTING - Start
                                SELECT COUNT(*) INTO COUNT_TEST_DATA_SETTING
                                            FROM tblstgtestcase stg
                                            INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                            INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                                AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                                AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                            INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                            INNER JOIN t_test_case_summary tcs ON UPPER(tcs.test_case_name) = UPPER(stg.testcasename)
                                            INNER JOIN T_TEST_STEPS tts ON tts.KEY_WORD_ID = tk.KEY_WORD_ID
                                                AND tts.RUN_ORDER = stg.ROWNUMBER
                                                AND tts.TEST_CASE_ID = tcs.test_case_id    
                                            INNER JOIN TEST_DATA_SETTING tdss ON tdss.STEPS_ID = tts.STEPS_ID
                                                AND tdss.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                            WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND stg.DATASETMODE IS NULL
                                                 and stg.KEYWORD IS NOT NULL ;
                                IF COUNT_TEST_DATA_SETTING !=0 THEN
                                  --dbms_output.put_line('@@@@@@@@@@');
                                    --dbms_output.put_line(COUNT_TEST_DATA_SETTING);   
                                    BEGIN
                                      /*
                                      MERGE INTO TEST_DATA_SETTING tdss1
                                        USING (
                                            SELECT DISTINCT tdss.DATA_SETTING_ID,stg.DATASETVALUE, tsop.OBJECT_POOL_ID,stg.skip
                                            FROM tblstgtestcase stg
                                            INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                            INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                                AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                                AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                            INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                            INNER JOIN t_test_case_summary tcs ON UPPER(tcs.test_case_name) = UPPER(stg.testcasename)
                                            INNER JOIN T_TEST_STEPS tts ON tts.KEY_WORD_ID = tk.KEY_WORD_ID
                                                AND tts.RUN_ORDER = stg.ROWNUMBER
                                                AND tts.TEST_CASE_ID = tcs.test_case_id    
                                            INNER JOIN TEST_DATA_SETTING tdss ON tdss.STEPS_ID = tts.STEPS_ID
                                                AND tdss.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                            WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND stg.DATASETMODE IS NULL
                                                 and stg.KEYWORD IS NOT NULL 
                                        ) TMP
                                        ON (tdss1.DATA_SETTING_ID = TMP.DATA_SETTING_ID)
                                        WHEN MATCHED THEN
                                        UPDATE SET
                                            tdss1.DATA_VALUE = TMP.DATASETVALUE,
                                            tdss1.POOL_ID = TMP.OBJECT_POOL_ID,
                                            tdss1.DATA_DIRECTION=TMP.skip ;
                                        */
                                        DECLARE
                                          CURSOR DATASETTINGCURSOR IS 
                                          SELECT DISTINCT tdss.DATA_SETTING_ID,tdss.DATA_VALUE,stg.DATASETVALUE, tdss.POOL_ID,tsop.OBJECT_POOL_ID,tdss.DATA_DIRECTION,stg.skip
                                          FROM tblstgtestcase stg
                                          INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                          INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                              AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                              AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                          INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                          INNER JOIN t_test_case_summary tcs ON UPPER(tcs.test_case_name) = UPPER(stg.testcasename)
                                          INNER JOIN T_TEST_STEPS tts ON tts.KEY_WORD_ID = tk.KEY_WORD_ID
                                              AND tts.RUN_ORDER = stg.ROWNUMBER
                                              AND tts.TEST_CASE_ID = tcs.test_case_id    
                                          INNER JOIN TEST_DATA_SETTING tdss ON tdss.STEPS_ID = tts.STEPS_ID
                                              AND tdss.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                          WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                              AND stg.DATASETMODE IS NULL
                                               and stg.KEYWORD IS NOT NULL;
                                      BEGIN
                                        FOR DATASETTIGN_INDEX IN DATASETTINGCURSOR
                                        LOOP
                                           UPDATE TEST_DATA_SETTING
                                            SET DATA_VALUE = DATASETTIGN_INDEX.DATASETVALUE,
                                              POOL_ID = DATASETTIGN_INDEX.OBJECT_POOL_ID,
                                              DATA_DIRECTION=DATASETTIGN_INDEX.skip
                                            WHERE DATA_SETTING_ID = DATASETTIGN_INDEX.DATA_SETTING_ID;
                                        END LOOP;    
                                      END;
                                    END;
                                    END IF;
                                -- Update TEST_DATA_SETTING - End

                                -- UPDATE t_test_data_summary - START

                                SELECT COUNT(*) INTO COUNT_DATA_SET_DESCRIPTION
                                FROM tblstgtestcase stg
                                INNER JOIN T_TEST_CASE_SUMMARY ttcs ON ttcs.TEST_CASE_NAME = stg.TESTCASENAME
                                INNER JOIN REL_TC_DATA_SUMMARY rel ON rel.TEST_CASE_ID = ttcs.TEST_CASE_ID
                                INNER JOIN t_test_data_summary tcs ON UPPER(tcs.ALIAS_NAME) = UPPER(stg.testcasename)
                                  AND tcs.DATA_SUMMARY_ID = rel.DATA_SUMMARY_ID
                                INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

                                IF COUNT_TEST_DATA_SETTING !=0 THEN
                                BEGIN
                                  MERGE INTO t_test_data_summary TTS1
                                  USING (
                                      SELECT DISTINCT tcs.DATA_SUMMARY_ID, stg.DATASETDESCRIPTION
                                      FROM tblstgtestcase stg
                                      INNER JOIN T_TEST_CASE_SUMMARY ttcs ON ttcs.TEST_CASE_NAME = stg.TESTCASENAME
                                      INNER JOIN REL_TC_DATA_SUMMARY rel ON rel.TEST_CASE_ID = ttcs.TEST_CASE_ID
                                      INNER JOIN t_test_data_summary tcs ON UPPER(tcs.ALIAS_NAME) = UPPER(stg.datasetname)
                                        AND tcs.DATA_SUMMARY_ID = rel.DATA_SUMMARY_ID
                                      WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                  ) TMP
                                  ON (TTS1.DATA_SUMMARY_ID = TMP.DATA_SUMMARY_ID)
                                  WHEN MATCHED THEN
                                  UPDATE SET
                                    TTS1.DESCRIPTION_INFO = tmp.DATASETDESCRIPTION;
                                END;
                                END IF;
                                -- UPDATE t_test_data_summary -END
                                -- All update Code ends
                                --dbms_output.put_line('All Update Ended');
                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                --dbms_output.put_line(CURRENTTIME);

                                --dbms_output.put_line('All Insert Started');
                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                --dbms_output.put_line(CURRENTTIME);
                                    -- Insert for new Test Suite -- Start
                                    --CHERISH
                                        --SELECT MAX(TEST_SUITE_ID) INTO MAXRow FROM T_TEST_SUITE;
                                        --remove distinct 
                                        SELECT T_TEST_SUITE_SEQ.nextval INTO MAXRow FROM DUAL;
                                        INSERT INTO T_TEST_SUITE (TEST_SUITE_ID,TEST_SUITE_NAME,TEST_SUITE_DESCRIPTION)
                                        SELECT   T_TEST_SUITE_SEQ.nextval ,TESTSUITENAME,Description
                                        FROM (
                                                SELECT DISTINCT tblstgtestcase.TESTSUITENAME,tblstgtestcase.TESTSUITENAME as Description
                                                FROM tblstgtestcase
                                                WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                    AND tblstgtestcase.DATASETMODE IS NULL 
                                                    and tblstgtestcase.KEYWORD IS NOT NULL 
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM T_TEST_SUITE
                                                        WHERE T_TEST_SUITE.TEST_SUITE_NAME = tblstgtestcase.TESTSUITENAME
                                                        AND ROWNUM=1
                                                    )
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM TEMPFALSEAPPLICATION
                                                        WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                            AND tempfalseapplication.FLAG = 0
                                                            AND ROWNUM=1
                                                    )
                                            );    
                                    -- Insert for new Test Suite -- END

                                    -- Insert for Test Suit Project Relation - start
                                    /*
                                        SELECT MAX(RELATIONSHIP_ID) INTO MAXRow FROM REL_TEST_SUIT_PROJECT;

                                        INSERT INTO REL_TEST_SUIT_PROJECT(RELATIONSHIP_ID,TEST_SUITE_ID,PROJECT_ID)
                                        SELECT DISTINCT MAXRow + ROWNUM AS ID,TEST_SUITE_ID,587
                                        FROM (
                                            SELECT DISTINCT T_TEST_SUITE.TEST_SUITE_ID
                                            FROM tblstgtestcase
                                            INNER JOIN T_TEST_SUITE ON T_TEST_SUITE.TEST_SUITE_NAME = tblstgtestcase.TESTSUITENAME
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM REL_TEST_SUIT_PROJECT
                                                    WHERE REL_TEST_SUIT_PROJECT.TEST_SUITE_ID = T_TEST_SUITE.TEST_SUITE_ID
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                )
                                        ); 
                                    */    
                                    -- Insert for Test Suit Project Relation - End

                                    -- Update Test Case -- Start
                                        UPDATE T_TEST_CASE_SUMMARY 
                                            SET T_TEST_CASE_SUMMARY.TEST_STEP_DESCRIPTION = (
                                                                                                SELECT DISTINCT tblstgtestcase.TESTCASEDESCRIPTION
                                                                                                FROM tblstgtestcase
                                                                                                WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                                                                    AND tblstgtestcase.DATASETMODE IS NULL 
                                                                                                     and tblstgtestcase.KEYWORD IS NOT NULL 
                                                                                                    AND NOT EXISTS (
                                                                                                        SELECT 1
                                                                                                        FROM TEMPFALSEAPPLICATION
                                                                                                        WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                                                            AND tempfalseapplication.FLAG = 0
                                                                                                            AND ROWNUM=1
                                                                                                    )
                                                                                                     AND ROWNUM=1
                                                                                            )
                                        WHERE t_test_case_summary.test_case_id IN   (
                                                                                        SELECT DISTINCT xy.test_case_id
                                                                                        FROM tblstgtestcase
                                                                                        INNER JOIN T_TEST_CASE_SUMMARY xy ON UPPER(xy.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                                                                        WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                                                            AND tblstgtestcase.DATASETMODE IS NULL 
                                                                                             and tblstgtestcase.KEYWORD IS NOT NULL 
                                                                                            AND NOT EXISTS (
                                                                                                SELECT 1
                                                                                                FROM TEMPFALSEAPPLICATION
                                                                                                WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                                                    AND tempfalseapplication.FLAG = 0
                                                                                                    AND ROWNUM=1
                                                                                            )
                                                                                             AND ROWNUM=1
                                                                                    );
                                    -- Update Test Case -- END

                                    -- Insert new Test Cases - Start
                                    --CHERISH
                                        --SELECT MAX(TEST_CASE_ID) INTO MAXRow FROM T_TEST_CASE_SUMMARY;    
                                        SELECT T_TEST_CASE_SUMMARY_SEQ.nextval INTO MAXRow FROM DUAL;    
                                        --remove distinct
                                        INSERT INTO T_TEST_CASE_SUMMARY (TEST_CASE_ID,TEST_CASE_NAME,TEST_STEP_DESCRIPTION,TEST_STEP_CREATOR,TEST_STEP_CREATE_TIME,USAGE_STATUS)
                                        SELECT  T_TEST_CASE_SUMMARY_SEQ.nextval AS ID,TESTCASENAME,TESTCASEDESCRIPTION,'SYSTEM',(SELECT SYSDATE FROM DUAL) AS CURRENTDATE,NULL
                                        FROM (
                                                SELECT DISTINCT tblstgtestcase.TESTCASENAME,tblstgtestcase.TESTCASEDESCRIPTION
                                                FROM tblstgtestcase
                                                WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                    AND tblstgtestcase.DATASETMODE IS NULL 
                                                     and tblstgtestcase.KEYWORD IS NOT NULL 
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM T_TEST_CASE_SUMMARY
                                                        WHERE T_TEST_CASE_SUMMARY.TEST_CASE_NAME = tblstgtestcase.TESTCASENAME
                                                        AND ROWNUM=1
                                                    )
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM TEMPFALSEAPPLICATION
                                                        WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                            AND tempfalseapplication.FLAG = 0
                                                            AND ROWNUM=1
                                                    )
                                        );        
                                    -- Insert new Test Cases - End

                                    -- Insert REL_TEST_CASE_TEST_SUITE -- Start
                                        INSERT INTO REL_TEST_CASE_TEST_SUITE (RELATIONSHIP_ID,TEST_CASE_ID,TEST_SUITE_ID)
                                        SELECT  (REL_TEST_CASE_TEST_SUITE_SEQ.NEXTVAL) AS ID, test_case_id, test_suite_id
                                        FROM ( 
                                            SELECT DISTINCT
                                                t_test_case_summary.test_case_id,
                                                t_test_suite.test_suite_id
                                            FROM tblstgtestcase
                                            INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN t_test_suite ON UPPER(t_test_suite.test_suite_name) = UPPER(tblstgtestcase.testsuitename)
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM REL_TEST_CASE_TEST_SUITE
                                                    WHERE REL_TEST_CASE_TEST_SUITE.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                        AND REL_TEST_CASE_TEST_SUITE.TEST_SUITE_ID = t_test_suite.test_suite_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                        ) xyz; 
                                    -- Insert REL_TEST_CASE_TEST_SUITE -- END

                                    -- Insert Test Data Set -- Start
                                    --CHERISH
                                        --SELECT MAX(DATA_SUMMARY_ID) INTO MAXRow FROM T_TEST_DATA_SUMMARY;
                                        SELECT T_TEST_STEPS_SEQ.nextval INTO MAXRow FROM DUAL;
                                        INSERT INTO T_TEST_DATA_SUMMARY(DATA_SUMMARY_ID,ALIAS_NAME,DESCRIPTION_INFO,AVAILABLE_MARK,VERSION,SHARE_MARK,CREATE_TIME,STATUS,DATA_SET_TYPE)
                                        SELECT  T_TEST_STEPS_SEQ.nextval AS ID,ALIAS_NAME,datasetdescription,AVAILABLE_MARK,VERSION,SHARE_MARK,CURRENTDATE,STATUS,DATA_SET_TYPE
                                        FROM (    
                                            SELECT DISTINCT 
                                                tblstgtestcase.DATASETNAME AS ALIAS_NAME,
                                                tblstgtestcase.datasetdescription,
                                                NULL AS AVAILABLE_MARK ,
                                                NULL AS VERSION,
                                                NULL AS SHARE_MARK,
                                                (SELECT SYSDATE FROM DUAL) AS CURRENTDATE,
                                                NULL AS STATUS,NULL as DATA_SET_TYPE
                                            FROM tblstgtestcase 
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM T_TEST_DATA_SUMMARY
                                                    WHERE t_test_data_summary.alias_name = tblstgtestcase.DATASETNAME
                                                    AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                            );    
                                    -- Insert Test Data Set -- End        

                                    -- Insert into REL_TC_DATA_SUMMARY - Start
                                    --CHERISH
                                        --SELECT MAX(ID) INTO MAXRow FROM REL_TC_DATA_SUMMARY;
                                        SELECT T_TEST_STEPS_SEQ.nextval INTO MAXRow FROM DUAL;

                                        INSERT INTO REL_TC_DATA_SUMMARY(ID,DATA_SUMMARY_ID,TEST_CASE_ID,CREATE_TIME)
                                        SELECT T_TEST_STEPS_SEQ.nextval AS ID,data_summary_id,test_case_id,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE
                                        FROM (    
                                            SELECT DISTINCT t_test_data_summary.data_summary_id,t_test_case_summary.test_case_id
                                            FROM tblstgtestcase
                                            INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                            INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM REL_TC_DATA_SUMMARY
                                                    WHERE REL_TC_DATA_SUMMARY.DATA_SUMMARY_ID = t_test_data_summary.data_summary_id
                                                        AND REL_TC_DATA_SUMMARY.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                        ); 
                                    -- Insert into REL_TC_DATA_SUMMARY - Start    

                                    -- Inser into Test Steps -- Start
                                    --CHERISH
                                        SELECT T_TEST_STEPS_SEQ.nextval INTO MAXRow FROM DUAL;
                                        --SELECT MAX(STEPS_ID)+1 INTO MAXRow FROM T_TEST_STEPS;

                                        INSERT INTO T_TEST_STEPS(STEPS_ID,RUN_ORDER,KEY_WORD_ID,TEST_CASE_ID,OBJECT_ID,COLUMN_ROW_SETTING,VALUE_SETTING,"COMMENT",IS_RUNNABLE,OBJECT_NAME_ID)
                                        SELECT T_TEST_STEPS_SEQ.nextval  AS ID,rownumber,key_word_id,test_case_id,object_id,parameter,NULL,comments,NULL,object_name_id
                                        FROM (    
                                            SELECT DISTINCT tblstgtestcase.rownumber,t_keyword.key_word_id,t_test_case_summary.test_case_id,t.object_id,tblstgtestcase.parameter,tblstgtestcase.comments,t.object_name_id
                                            FROM tblstgtestcase
                                            INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                            INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgtestcase.KEYWORD)
                                            CROSS JOIN TABLE(GETTOPOBJECT(tblstgtestcase.OBJECT, APPLICATIONID)) t
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM T_TEST_STEPS
                                                    WHERE T_TEST_STEPS.KEY_WORD_ID = t_keyword.key_word_id
                                                        AND T_TEST_STEPS.RUN_ORDER = tblstgtestcase.rownumber
                                                        AND T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                            ORDER BY t_test_case_summary.test_case_id,tblstgtestcase.rownumber    
                                        ); 
                                    -- Inser into Test Steps -- End

                                    -- Insert into T_SHARED_OBJECT_POOL -- Start
                                        SELECT MAX(OBJECT_POOL_ID) INTO MAXRow FROM  T_SHARED_OBJECT_POOL;

                                        INSERT INTO T_SHARED_OBJECT_POOL (OBJECT_POOL_ID,DATA_SUMMARY_ID,OBJECT_NAME,OBJECT_ORDER,LOOP_ID,DATA_VALUE,CREATE_TIME,VERSION)
                                        SELECT T_TEST_STEPS_SEQ.nextval AS ID,data_summary_id,object,rownumber,1,datasetvalue,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE, NULL
                                        FROM (    
                                            SELECT DISTINCT t_test_data_summary.data_summary_id,tblstgtestcase.object,tblstgtestcase.rownumber,tblstgtestcase.datasetvalue
                                            FROM tblstgtestcase
                                            INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                            INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgtestcase.KEYWORD)
                                            CROSS JOIN TABLE(GETTOPOBJECT(tblstgtestcase.OBJECT, APPLICATIONID)) t
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM T_SHARED_OBJECT_POOL
                                                    WHERE T_SHARED_OBJECT_POOL.DATA_SUMMARY_ID = t_test_data_summary.data_summary_id
                                                        AND NVL(T_SHARED_OBJECT_POOL.OBJECT_NAME, 'N/A') = NVL(tblstgtestcase.object, 'N/A')
                                                        AND T_SHARED_OBJECT_POOL.OBJECT_ORDER = tblstgtestcase.rownumber
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                                ORDER BY t_test_data_summary.data_summary_id, tblstgtestcase.rownumber
                                        ); 
                                    -- Insert into T_SHARED_OBJECT_POOL -- Start    

                                    -- Insert into TEST_DATA_SETTING - Start
                                    --CHERISH
                                       -- SELECT MAX(DATA_SETTING_ID) INTO MAXRow FROM TEST_DATA_SETTING;
                                        SELECT TEST_DATA_SETTING_SEQ.nextval INTO MAXRow FROM DUAL;

                                        INSERT INTO TEST_DATA_SETTING(DATA_SETTING_ID,STEPS_ID,LOOP_ID,DATA_VALUE,VALUE_OR_OBJECT,DESCRIPTION,DATA_SUMMARY_ID,DATA_DIRECTION,VERSION,CREATE_TIME,POOL_ID)
                                        SELECT  TEST_DATA_SETTING_SEQ.nextval AS ID,steps_id,1,datasetvalue,NULL,NULL,data_summary_id,skip,NULL,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE,object_pool_id
                                        FROM (    
                                            SELECT DISTINCT t_test_steps.steps_id,tblstgtestcase.datasetvalue,t_test_data_summary.data_summary_id,t_shared_object_pool.object_pool_id,tblstgtestcase.skip
                                            FROM tblstgtestcase
                                            INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                            INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgtestcase.KEYWORD)
                                            INNER JOIN T_SHARED_OBJECT_POOL ON T_SHARED_OBJECT_POOL.DATA_SUMMARY_ID = t_test_data_summary.data_summary_id
                                                AND NVL(T_SHARED_OBJECT_POOL.OBJECT_NAME,'N/A') = NVL(tblstgtestcase.object, 'N/A')
                                                AND T_SHARED_OBJECT_POOL.OBJECT_ORDER = tblstgtestcase.rownumber
                                            INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.KEY_WORD_ID = t_keyword.key_word_id
                                                AND T_TEST_STEPS.RUN_ORDER = tblstgtestcase.rownumber
                                                AND T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.test_case_id
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEST_DATA_SETTING
                                                    WHERE test_data_setting.steps_id = t_test_steps.steps_id
                                                        AND TEST_DATA_SETTING.DATA_SUMMARY_ID = t_test_data_summary.data_summary_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                                ORDER BY t_test_steps.steps_id
                                        ); 
                                    -- Insert into TEST_DATA_SETTING - End


                                    -- Insert into REL_APP_TESTCASE - Start
                                        SELECT MAX(RELATIONSHIP_ID) INTO MAXRow FROM REL_APP_TESTCASE;

                                        INSERT INTO REL_APP_TESTCASE(RELATIONSHIP_ID,APPLICATION_ID,TEST_CASE_ID)
                                        --VALUES(COUNT_REL_APP_TESTCASE,APPLICATIONID,GETTESTCASEID);
                                        SELECT REL_APP_TESTCASE_SEQ.nextval AS ID, application_id, test_case_id
                                        FROM (    
                                            SELECT DISTINCT t_registered_apps.application_id, t_test_case_summary.test_case_id
                                            FROM tblstgtestcase 
                                            INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN TEMPFALSEAPPLICATION ON UPPER(tempfalseapplication.testcasename) = UPPER(tblstgtestcase.testcasename)
                                                AND tempfalseapplication.FLAG = 1
                                            INNER JOIN T_REGISTERED_APPS ON t_registered_apps.app_short_name = tempfalseapplication.application
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM REL_APP_TESTCASE
                                                    WHERE REL_APP_TESTCASE.APPLICATION_ID = t_registered_apps.application_id
                                                        AND REL_APP_TESTCASE.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0 
                                                        AND ROWNUM=1
                                                )
                                        );     
                                    -- Insert into REL_APP_TESTCASE - End


                                    -- Insert into REL_APP_TESTSUITE - Start
                                        SELECT MAX(RELATIONSHIP_ID)+1 INTO MAXRow FROM REL_APP_TESTSUITE;

                                        INSERT INTO REL_APP_TESTSUITE(RELATIONSHIP_ID,APPLICATION_ID,TEST_SUITE_ID)
                                        --VALUES(COUNT_REL_APP_TESTCASE,APPLICATIONID,GETTESTCASEID);
                                        SELECT REL_APP_TESTSUITE_SEQ.nextval AS ID, application_id, test_suite_id
                                        FROM (    
                                            SELECT DISTINCT t_registered_apps.application_id, t_test_suite.test_suite_id
                                            --SELECT 1
                                            FROM tblstgtestcase 
                                            INNER JOIN t_test_suite ON UPPER(t_test_suite.test_suite_name) = UPPER(tblstgtestcase.testsuitename)
                                            INNER JOIN TEMPFALSEAPPLICATION ON UPPER(tempfalseapplication.testcasename) = UPPER(tblstgtestcase.testcasename)
                                                AND tempfalseapplication.FLAG = 1
                                            INNER JOIN T_REGISTERED_APPS ON t_registered_apps.app_short_name = tempfalseapplication.application
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM REL_APP_TESTSUITE
                                                    WHERE REL_APP_TESTSUITE.APPLICATION_ID = t_registered_apps.application_id
                                                        AND rel_app_testsuite.test_suite_id = t_test_suite.test_suite_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0 
                                                        AND ROWNUM=1
                                                )
                                        );
                                    -- Insert into REL_APP_TESTSUITE - Start    
                                --dbms_output.put_line('All Insert Ended');
                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                --dbms_output.put_line(CURRENTTIME);
                            END;    

                            -- Performance Code -- Shivam - End

                        -- Code added by Shivam - To save the errorlog for tab with wrong application - Start
                            execute immediate 'TRUNCATE TABLE TEMP1';
                            execute immediate 'TRUNCATE TABLE TEMPSTGTESTCASE';
                            execute immediate 'TRUNCATE TABLE TEMPDWTESTCASE';

					--END; --- LOGIC FOR NOT OVERWRITE - END
---------------------------------------------------------------------------------------------------------- LOGIC FOR NOT OVERWRITE - END ----------------------------------------------------------------------------------------------------------
				--END IF; -- IF - 3 - END

				-------------- LOGIC FOR ANOTHER CURSOR WHERE DATASETMODE IS NOT NULL  --- START-----------------------------------

                -- Performance change by Shivam Parikh - END
-------------------------------------------------------------------------------
--------------------LOGIC FOR MODE D - START ----------------------------------

            -- Performance Code by Shivam Parikh - Start
            DECLARE 
                ERROR varchar2(5000);
                MESSAGE VARCHAR2(500);
                MaxRow INT;
            BEGIN

                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=17;

                -- Insert log for Test case and Test Date which are going to be deleted - Start
                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,xyz.LOGDETAILS,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                FROM (
                        SELECT DISTINCT T_TEST_CASE_SUMMARY.TEST_CASE_NAME ||','|| t_test_data_summary.alias_name || ',' || T_KEYWORD.KEY_WORD_NAME || ',' || T_REGISTED_OBJECT.OBJECT_HAPPY_NAME as LOGDETAILS,(SELECT SYSDATE FROM DUAL) as CREATEDON
                        FROM tblstgtestcase
                        INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name) = UPPER(TBLSTGTESTCASE.DATASETNAME)
                        INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(TEST_CASE_NAME)=UPPER(TBLSTGTESTCASE.TESTCASENAME)
                        INNER JOIN T_Test_Steps ON T_Test_Steps.TEST_CASE_ID = T_TEST_CASE_SUMMARY.TEST_CASE_ID
                        INNER JOIN T_KEYWORD ON T_KEYWORD.KEY_WORD_ID=T_Test_Steps.KEY_WORD_ID
                        INNER JOIN T_REGISTED_OBJECT ON T_REGISTED_OBJECT.OBJECT_ID=T_Test_Steps.OBJECT_ID
                        WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID 
                            AND DATASETMODE='D'

                ) xyz;
                -- Insert log for Test case and Test Date which are going to be deleted - Start

                -- Insert log for not found Test Case - Start
                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=14;

                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,MESSAGE,LOGDETAIL,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                FROM (    
                    SELECT DISTINCT tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.DATASETNAME AS LOGDETAIL
                    FROM tblstgtestcase
                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                        AND DATASETMODE='D'
                        AND NOT EXISTS (
                            SELECT 1
                            FROM T_TEST_CASE_SUMMARY
                            WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(tblstgtestcase.TESTCASENAME)
                        )
                ) xyz;
                -- Insert log for not found Test Case - End

                -- Insert log for not found Data Summary - Start

                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=15;

                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,MESSAGE,LOGDETAIL,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                FROM (    
                    SELECT DISTINCT tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.DATASETNAME AS LOGDETAIL
                    FROM tblstgtestcase
                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                        AND DATASETMODE='D'
                        AND NOT EXISTS (
                            SELECT 1
                            FROM T_TEST_DATA_SUMMARY
                            WHERE UPPER(t_test_data_summary.alias_name)=UPPER(tblstgtestcase.DATASETNAME)
                            AND ROWNUM=1
                        )
                ) xyz; 
                -- Insert log for not found Data Summary - End

                -- Insert log for not found Data Summary and Data Set both - Start

                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=16;

                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,MESSAGE,LOGDETAIL,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                FROM (    
                    SELECT DISTINCT tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.DATASETNAME AS LOGDETAIL
                    FROM tblstgtestcase
                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                        AND DATASETMODE='D'
                        AND NOT EXISTS (
                            SELECT 1
                            FROM T_TEST_DATA_SUMMARY
                            WHERE UPPER(t_test_data_summary.alias_name)=UPPER(tblstgtestcase.DATASETNAME)
                            AND ROWNUM=1
                        )
                        AND NOT EXISTS (
                            SELECT 1
                            FROM T_TEST_CASE_SUMMARY
                            WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(tblstgtestcase.TESTCASENAME) 
                            AND ROWNUM=1
                        )
                ) xyz; 
                -- Insert log for not found Data Summary and Data Set both - Start

                -- DELETE From tables - Start
                DELETE FROM TEST_DATA_SETTING
                WHERE TEST_DATA_SETTING.STEPS_ID IN 
                (
                    SELECT DISTINCT T_TEST_STEPS.STEPS_ID
                    FROM TBLSTGTESTCASE
                    INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name)=UPPER(TBLSTGTESTCASE.DATASETNAME)        
                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(TEST_CASE_NAME)=UPPER(TBLSTGTESTCASE.TESTCASENAME)
                    INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.TEST_CASE_ID=T_TEST_CASE_SUMMARY.TEST_CASE_ID
                    INNER JOIN test_data_setting ON test_data_setting.data_summary_id = t_test_data_summary.data_summary_id
                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                        AND DATASETMODE='D'
                );

                DELETE FROM REL_TC_DATA_SUMMARY
                WHERE REL_TC_DATA_SUMMARY.ID IN 
                (
                    SELECT DISTINCT REL_TC_DATA_SUMMARY.ID
                    FROM TBLSTGTESTCASE
                    INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name)=UPPER(TBLSTGTESTCASE.DATASETNAME)
                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(TEST_CASE_NAME)=UPPER(TBLSTGTESTCASE.TESTCASENAME)
                    INNER JOIN REL_TC_DATA_SUMMARY ON REL_TC_DATA_SUMMARY.TEST_CASE_ID = T_TEST_CASE_SUMMARY.TEST_CASE_ID
                        AND REL_TC_DATA_SUMMARY.DATA_SUMMARY_ID = T_TEST_DATA_SUMMARY.DATA_SUMMARY_ID
                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                        AND DATASETMODE='D'
                );
                -- DELETE From tables - End

            END;
            -- Performance Code by Shivam Parikh - End

-------------------------------------------------------------------------------
-------------------LOGIC FOR MODE D - END -------------------------------------







							-------------- LOGIC FOR ANOTHER CURSOR WHERE DATASETMODE IS NOT NULL  --- END-----------------------------------







							END;
							ELSE
								DECLARE
									AUTO_LOGREPORT NUMBER:=0;
								BEGIN
									SELECT MESSAGE INTO VALIDATEMESSAGE FROM TBLMESSAGE WHERE ID=11;
									SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

									INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
									VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,VALIDATEMESSAGE,'',AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
										IF RESULT IS NULL THEN
											RESULT:= 'ERROR';
										ELSE
											RESULT:= RESULT || '####' || 'ERROR';
										END IF;
								END;
						END IF; -- IF - 2 - END

									END;
								ELSE
									DECLARE
											AUTO_LOGREPORT NUMBER:=0;
									BEGIN

											-- ERROR MESSAGE OF APPLICATION ID IS NOT FOUND IN DATABASE
											SELECT MESSAGE INTO VALIDATEMESSAGE FROM TBLMESSAGE WHERE ID=12;
											SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

											INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
											VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,VALIDATEMESSAGE,'',AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
												IF RESULT IS NULL THEN
													RESULT:= 'ERROR';
												ELSE
													RESULT:= RESULT || '####' || 'ERROR';
												END IF;
									END;
								END IF;

							END;
						END IF;





------------------------------------------------------- END - CODE FOR IMPORT TEST CASE ---------------------------------
--------------------------------------------------------------------------------------------------------------------------


----------------------------------------------------- START - CODE FOR IMPORT Storyboard ---------------------------------
-------------------------------------------------------------------------------------------------------------------------
 IF I_INDEX.FILETYPE = 'STORYBOARD' THEN
          DECLARE
            CURRENTDATE TIMESTAMP;
        BEGIN
		SELECT SYSDATE INTO CURRENTDATE FROM DUAL;  
         insert into Storyboard_Insert_Temp (Name,Type)        
        select distinct regexp_substr(applicationname,'[^,]+', 1, level), 'APPLICATION' from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
        connect by regexp_substr(applicationname, '[^,]+', 1, level) is not null; 

        insert into Storyboard_Insert_Temp(Name,Type,Description)
        select distinct projectname,'PROJECT',projectdetail from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

        insert into Storyboard_Insert_Temp(Name,Type,Description)
        select distinct storyboardname,'STORYBOARD',projectname from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

        insert into Storyboard_Insert_Temp (Name,Type,Description)        
        select distinct regexp_substr(applicationname,'[^,]+', 1, level), 'Mapping',projectname from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
        connect by regexp_substr(applicationname, '[^,]+', 1, level) is not null; 

        insert into Storyboard_Insert_Temp(Name,Type,Description)
        select distinct suitename,'MappingTestSuiteProject',projectname from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;




         update t_test_project  tt
         set  tt.project_description = (select  temp.description from Storyboard_Insert_Temp temp where temp.name = tt.project_name and temp.Type = 'PROJECT' and ROWNUM = 1
)
         where Exists (select temp.name from Storyboard_Insert_Temp temp where temp.name = tt.project_name and temp.Type = 'PROJECT' and ROWNUM = 1);





            insert into t_test_project(Project_Id,project_name,project_description,creator,create_date,status)
             select T_TEST_PROJECT_SEQ.nextval,temp.name,temp.description,'System',CURRENTDATE,2 from Storyboard_Insert_Temp temp
            left join t_test_project tt on temp.name = tt.project_name
            where tt.project_id is null and temp.Type = 'PROJECT';




             insert into t_storyboard_summary(storyboard_id,assigned_project_id,storyboard_name)
             select T_TEST_STEPS_SEQ.nextval,  prj.project_id,temp.name from Storyboard_Insert_Temp temp
              join t_test_project prj on prj.project_name = temp.description 
            left join t_storyboard_summary tt on temp.name = tt.storyboard_name and prj.project_id = tt.assigned_project_id

            where tt.storyboard_id is null and temp.Type = 'STORYBOARD';



            insert into REL_APP_PROJ(relationship_id,application_id,project_id)         
         select REL_APP_PROJ_SEQ.nextval,apps.application_id,prj.project_id from Storyboard_Insert_Temp stg
         join t_test_project prj on prj.project_name = stg.description
         join t_registered_apps apps on apps.app_short_name = stg.name
         left join REL_APP_PROJ rels on rels.application_id = apps.application_id and rels.project_id = prj.project_id
         where rels.relationship_id is null and
         stg.type = 'Mapping';


          insert into REL_TEST_SUIT_PROJECT(relationship_id,test_suite_id,project_id)         
         select T_TEST_STEPS_SEQ.nextval,suite.test_suite_id,prj.project_id from Storyboard_Insert_Temp stg
         join t_test_project prj on prj.project_name = stg.description
         join t_test_suite suite on suite.test_suite_name = stg.name
         left join REL_TEST_SUIT_PROJECT rels on rels.test_suite_id = suite.test_suite_id and rels.project_id = prj.project_id
         where rels.relationship_id is null and
         stg.type = 'MappingTestSuiteProject';





         delete T_STORYBOARD_DATASET_SETTING where storyboard_detail_id in
         (select mgr.storyboard_detail_id from tblstgstoryboard sa 
            join t_test_project prj  on sa.projectname = prj.project_name
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
          join T_PROJ_TC_MGR mgr on mgr.storyboard_id = tc.storyboard_id and mgr.project_id= prj.project_id
          where sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID 
         ) ;

         --delete from T_PROJ_TC_MGR
         delete T_PROJ_TC_MGR where storyboard_detail_id in
         (select mgr.storyboard_detail_id from tblstgstoryboard sa 
            join t_test_project prj  on sa.projectname = prj.project_name
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
          join T_PROJ_TC_MGR mgr on mgr.storyboard_id = tc.storyboard_id and mgr.project_id= prj.project_id
          where sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID 
         ) ;


           insert into  T_PROJ_TC_MGR(storyboard_detail_id,project_id,test_case_id,storyboard_id,run_type,run_order,test_suite_id,alias_name)
            select T_TEST_STEPS_SEQ.nextval ,prj.project_id,tcs.test_case_id,tc.storyboard_id,lkp.value,sa.runorder,trs.test_suite_id,sa.stepname from tblstgstoryboard sa                        
            join t_test_suite trs on trs.test_suite_name = sa.suitename
            join t_test_case_summary tcs on tcs.test_case_name = sa.casename
            join t_test_project prj  on sa.projectname = prj.project_name      

            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on lkp.display_name = sa.actionname and lkp.field_name = 'RUN_TYPE'
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID ;

            update T_PROJ_TC_MGR mgr
            set mgr.depends_on = (select mgr2.storyboard_detail_id from tblstgstoryboard sa                        
            join t_test_suite trs on trs.test_suite_name = sa.suitename
            join t_test_case_summary tcs on tcs.test_case_name = sa.casename
            join t_test_project prj  on sa.projectname = prj.project_name      
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on lkp.display_name = sa.actionname and lkp.field_name = 'RUN_TYPE'            
            join T_PROJ_TC_MGR mgr2 on tc.storyboard_id = mgr2.storyboard_id and prj.project_id = mgr2.project_id and mgr2.alias_name = sa.dependency
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
            and tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id and trs.test_suite_id = mgr.test_suite_id 
                        and tcs.test_case_id = mgr.test_case_id  and mgr.run_type = lkp.value and sa.runorder = mgr.run_order
            )
            where exists (select mgr2.storyboard_detail_id from tblstgstoryboard sa                        
            join t_test_suite trs on trs.test_suite_name = sa.suitename
            join t_test_case_summary tcs on tcs.test_case_name = sa.casename
            join t_test_project prj  on sa.projectname = prj.project_name      
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on lkp.display_name = sa.actionname and lkp.field_name = 'RUN_TYPE'            
            join T_PROJ_TC_MGR mgr2 on tc.storyboard_id = mgr2.storyboard_id and prj.project_id = mgr2.project_id and mgr2.alias_name = sa.dependency
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
            and tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id and trs.test_suite_id = mgr.test_suite_id 
                        and tcs.test_case_id = mgr.test_case_id  and mgr.run_type = lkp.value and sa.runorder = mgr.run_order);            





           insert into  T_STORYBOARD_DATASET_SETTING(setting_id,  storyboard_detail_id,data_summary_id)
            select T_TEST_STEPS_SEQ.nextval ,mgr.storyboard_detail_id,tr.data_summary_id from tblstgstoryboard sa                        
            join t_test_data_summary tr on tr.alias_name = sa.datasetname
            join t_test_suite trs on trs.test_suite_name = sa.suitename
            join t_test_case_summary tcs on tcs.test_case_name = sa.casename
            join t_test_project prj  on sa.projectname = prj.project_name
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on lkp.display_name = sa.actionname and lkp.field_name = 'RUN_TYPE'
            join T_PROJ_TC_MGR mgr on tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id and trs.test_suite_id = mgr.test_suite_id 
                        and tcs.test_case_id = mgr.test_case_id
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID ;


        End;             
        End If;
        commit;

------------------------------------------------------- END - CODE FOR IMPORT Storyboard ---------------------------------
--------------------------------------------------------------------------------------------------------------------------


----------------------------------------------------- START - CODE FOR IMPORT Variable ---------------------------------
-------------------------------------------------------------------------------------------------------------------------

IF I_INDEX.FILETYPE = 'VARIABLE' THEN
          DECLARE
            CURRENTDATE TIMESTAMP;
        --    ApplicationName VARCHAR2(5000);
        -- Remove duplicate staging rows
        BEGIN
        DELETE FROM tblstgvariable
           WHERE ID IN (
             SELECT DISTINCT ID
             from tblstgvariable t
             where t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
             AND EXISTS (
               SELECT 1
               FROM tblstgvariable s
               WHERE s.ID > t.ID
               AND s.feedprocessdetailid = t.feedprocessdetailid
               AND (               
              s.feedprocessdetailid = s.feedprocessdetailid 
              and NVL(s.name, 'N/A') = NVL(t.name, 'N/A')
              and NVL(s.type, 'N/A') = NVL(t.type, 'N/A')              
              and NVL(s.base_comp, 'N/A') = NVL(t.base_comp, 'N/A')
               )
            )
        );    
        commit;    


         --region Project
        -- update Global Variable  and Loop/Modal Variable     
         update system_lookup  lkp
         set  lkp.display_name = (select upper(temp.value) from tblstgvariable temp where upper(temp.name) = upper(lkp.field_name) and upper(temp.type) = 'GLOBAL_VAR' and temp.Feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID )
         where Exists (select temp.value from tblstgvariable temp where upper(temp.name) = upper(lkp.field_name) and upper(temp.type) = 'GLOBAL_VAR' and temp.Feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID)
         and lkp.table_name = 'GLOBAL_VAR';

         update system_lookup  lkp
         set  lkp.display_name = (select upper(temp.value) from tblstgvariable temp where upper(temp.name) = upper(lkp.field_name) and upper(temp.type) in ('MODAL_VAR','LOOP_VAR') 
                and temp.base_comp_id = lkp.status and temp.Feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID)
         where Exists (select temp.value from tblstgvariable temp where upper(temp.name) = upper(lkp.field_name) and upper(temp.type) in ('MODAL_VAR','LOOP_VAR')
                and temp.base_comp_id = lkp.status and temp.Feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID)
         and lkp.table_name in ('MODAL_VAR','LOOP_VAR') ;

         -- insert into Variable 

            insert into system_lookup(lookup_id,table_name,field_name,value,display_name,status)           
            select SYSTEM_LOOKUP_SEQ.nextval ,upper(temp.type) ,upper(temp.name),1,upper(temp.value) ,temp.base_comp_id from tblstgvariable temp
            left join system_lookup lkp on upper(temp.name) = upper(lkp.field_name) and lkp.table_name in ('MODAL_VAR','LOOP_VAR','GLOBAL_VAR')
            where lkp.lookup_id is null  and upper(temp.Type) in ('MODAL_VAR','LOOP_VAR','GLOBAL_VAR')
            and temp.Feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

         -- endregion for Variable


         --endregion
        End;             
        End If;------------------------------------------------------- END - CODE FOR IMPORT Variable ---------------------------------
--------------------------------------------------------------------------------------------------------------------------

      -- Remove all duplicate Data set mappings in the import - Start
        DELETE FROM REL_TC_DATA_SUMMARY
        WHERE ID IN (
          SELECT DISTINCT
            rdup.ID
          FROM REL_TC_DATA_SUMMARY r
          INNER JOIN REL_TC_DATA_SUMMARY rdup ON r.TEST_CASE_ID = r.TEST_CASE_ID
            AND r.DATA_SUMMARY_ID = r.DATA_SUMMARY_ID
            AND rdup.ID > r.ID
          INNER JOIN T_TEST_DATA_SUMMARY ts ON ts.DATA_SUMMARY_ID = r.DATA_SUMMARY_ID
          INNER JOIN T_TEST_DATA_SUMMARY tsdup ON tsdup.DATA_SUMMARY_ID = rdup.DATA_SUMMARY_ID
            AND ts.ALIAS_NAME = tsdup.ALIAS_NAME
            AND NVL(ts.DESCRIPTION_INFO, 'N/A') = NVL(ts.DESCRIPTION_INFO, 'N/A')
          INNER JOIN T_TEST_CASE_SUMMARY tc ON tc.TEST_CASE_ID = r.TEST_CASE_ID
          INNER JOIN T_TEST_CASE_SUMMARY tcdup ON tcdup.TEST_CASE_ID = rdup.TEST_CASE_ID
            AND tc.TEST_CASE_NAME = tc.TEST_CASE_NAME
          INNER JOIN REL_TEST_CASE_TEST_SUITE rtcts ON rtcts.TEST_CASE_ID = tc.TEST_CASE_ID
            AND rtcts.TEST_CASE_ID = tcdup.TEST_CASE_ID  
          INNER JOIN T_TEST_SUITE tss ON tss.TEST_SUITE_ID = rtcts.TEST_SUITE_ID
          INNER JOIN tblstgtestcase stg ON stg.TESTSUITENAME = tss.TEST_SUITE_NAME
          INNER JOIN TBLFEEDPROCESSDETAILS fpd ON fpd.FEEDPROCESSDETAILID = stg.FEEDPROCESSDETAILID
            AND fpd.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
          WHERE   rdup.ID > r.ID
        );
      -- Remove all duplicate Data set mappings in the import - End


			END LOOP;
	--CLOSE FORTBLFEEDPROCESS;
END;
--execute immediate 'TRUNCATE TABLE tblstgtestcase';
BEGIN
    EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_LAST_TC_INFO COMPILE';
    EXCEPTION
    WHEN OTHERS THEN 
    EXECUTE IMMEDIATE 'ALTER VIEW MV_LAST_TC_INFO COMPILE';
END;   
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_LATEST_STB_TSMOD_MARK  COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_MARS_STB_SNAPSHOT_SUB COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_STORYBOARD_LATEST COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_TC_DATASUMMARY COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW V_OBJECT_SNAPSHOT COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_OBJ_WITH_PEG COMPILE';

END USP_FEEDPROCESSMAPPING_Mode_B;
/

create or replace PROCEDURE USP_FEEDPROCESSMAPPING_Mode_D(
    FEEDPROCESSID1 IN NUMBER,
   ISOVERWRITE IN NUMBER,
    RESULT OUT CLOB
)
IS 
BEGIN
		DECLARE


			CURSOR FORTBLFEEDPROCESS IS 
		    SELECT  FEEDPROCESSDETAILID,FEEDPROCESSID,FILENAME,FEEDPROCESSSTATUS,CREATEDBY,CREATEDON,FILETYPE FROM TBLFEEDPROCESSDETAILS WHERE FEEDPROCESSID = FEEDPROCESSID1;
BEGIN
			--OPEN FORTBLFEEDPROCESS;

			FOR I_INDEX IN FORTBLFEEDPROCESS
			LOOP

						---------------------------- START - CODE FOR IMPORT OBJECT FILE ---------------------------------
						--------------------------------------------------------------------------------------------------
						IF I_INDEX.FILETYPE = 'OBJECT' THEN

							DECLARE

							GETTYPEID NUMBER:=0; -- USE IN IMPORT OBJECT
							NEWAPPLICATION_ID NUMBER:=0;
							FULLDATA CLOB;
							GETOBJECTHAPPYNAME VARCHAR2(500):=NULL;
							GETOBJECTNAMEID NUMBER;
							GETOBJECTPARENT VARCHAR2(500):=NULL;
              PARENTOBJECTEXISTS NUMERIC:=0;
							GETOBJECTPARENTID NUMBER;
              CURRENTOBJECTEXISTS NUMERIC:=0;
							CURRENTDATE TIMESTAMP;

							CURSOR FORTBLSTGGUIOBJECT IS
							SELECT OBJECTNAME,TYPE,QUICKACCESS,PARENT,OBJECTCOMMENT,ENUMTYPE,SQL,APPLICATIONNAME,FEEDPROCESSDETAILID FROM TBLSTGGUIOBJECT WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

							BEGIN

									FOR OBJ_INDEX IN FORTBLSTGGUIOBJECT
									LOOP

									   SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

										 FULLDATA := (OBJ_INDEX.OBJECTNAME || '####' || OBJ_INDEX.TYPE || '####' || OBJ_INDEX.QUICKACCESS || '####' || OBJ_INDEX.PARENT || '####' || OBJ_INDEX.OBJECTCOMMENT || '####' || OBJ_INDEX.ENUMTYPE || '####' || OBJ_INDEX."SQL" || '####' || OBJ_INDEX.APPLICATIONNAME || '####' || OBJ_INDEX.FEEDPROCESSDETAILID); 

										 -- CHECK APPLICATION ID IS EXSITS OR NOT
										 SELECT APPLICATION_ID INTO NEWAPPLICATION_ID FROM T_REGISTERED_APPS WHERE UPPER(APP_SHORT_NAME)=UPPER(OBJ_INDEX.APPLICATIONNAME) ; -- TO GET THE NEXT VALUE

                    --dbms_output.put_line('Application ID: ');
                    --dbms_output.put(NEWAPPLICATION_ID);
										 IF NEWAPPLICATION_ID != 0 THEN

												BEGIN
													--- CHECK TYPE_ID IS IN TABLE T_GUI_COMPONENT_TYPE_DIC IF NOT FOUND THEN STOP

													SELECT TYPE_ID  INTO GETTYPEID FROM T_GUI_COMPONENT_TYPE_DIC WHERE UPPER(TYPE_NAME) = UPPER(OBJ_INDEX.TYPE);
													IF  GETTYPEID != 0 THEN
														BEGIN

																--- CHECK PARENT IN TABLE : T_OBJECT_NAMEINFO IF NOT THEN CREATE AND GET THAT ID AND NAME VALUE PAIR
                                --dbms_output.put_line(OBJ_INDEX.PARENT);
																SELECT COUNT(1) INTO PARENTOBJECTEXISTS FROM T_OBJECT_NAMEINFO WHERE OBJECT_HAPPY_NAME=OBJ_INDEX.PARENT AND ROWNUM = 1;

																IF PARENTOBJECTEXISTS = 0 THEN
																	BEGIN





																			INSERT INTO "T_OBJECT_NAMEINFO" (OBJECT_NAME_ID,OBJECT_HAPPY_NAME,PEGWINDOW_MARK,OBJNAME_DESCRIPTION)
																			VALUES(SEQ_MARS_OBJECT_ID.nextval,OBJ_INDEX.OBJECTNAME,0,NULL);

                                                                            SELECT MAX(OBJECT_NAME_ID)  INTO GETOBJECTPARENTID FROM T_OBJECT_NAMEINFO;

                                                                            SELECT OBJECT_HAPPY_NAME  INTO GETOBJECTPARENT FROM T_OBJECT_NAMEINFO WHERE T_OBJECT_NAMEINFO.OBJECT_NAME_ID=GETOBJECTPARENTID;
                                      --dbms_output.put_line('New Object--');
                                      --dbms_output.put(GETOBJECTPARENTID);
                                      --SYS.DBMS_OUTPUT.PUT('--' || GETOBJECTPARENT);
																	END;
                                ELSE BEGIN
                                  SELECT OBJECT_NAME_ID  INTO GETOBJECTPARENTID FROM T_OBJECT_NAMEINFO WHERE OBJECT_HAPPY_NAME=OBJ_INDEX.PARENT AND ROWNUM = 1;
                                  SELECT OBJECT_HAPPY_NAME  INTO GETOBJECTPARENT FROM T_OBJECT_NAMEINFO WHERE T_OBJECT_NAMEINFO.OBJECT_NAME_ID=GETOBJECTPARENTID;
                                  --dbms_output.put_line('Existing Object--');
                                  --dbms_output.put(GETOBJECTPARENTID);
                                  --SYS.DBMS_OUTPUT.PUT('--' || GETOBJECTPARENT);
                                END;
																END IF;


																--- CHECK OBJECT_HAPPY_NAME IN TABLE : T_OBJECT_NAMEINFO IF NOT THEN CREATE AND GET THAT ID AND NAME VALUE PAIR

																SELECT COUNT(1) INTO CURRENTOBJECTEXISTS 
                                FROM T_OBJECT_NAMEINFO 
                                --INNER JOIN T_REGISTED_OBJECT ON T_REGISTED_OBJECT.OBJECT_NAME_ID = T_OBJECT_NAMEINFO.OBJECT_NAME_ID
                                  --AND T_REGISTED_OBJECT.OBJECT_TYPE = OBJ_INDEX.PARENT
                                WHERE T_OBJECT_NAMEINFO.OBJECT_HAPPY_NAME=OBJ_INDEX.OBJECTNAME
                                  --AND T_REGISTED_OBJECT.APPLICATION_ID = NEWAPPLICATION_ID
                                  ;

																IF CURRENTOBJECTEXISTS = 0 THEN
																	BEGIN


																			INSERT INTO "T_OBJECT_NAMEINFO" (OBJECT_NAME_ID,OBJECT_HAPPY_NAME,PEGWINDOW_MARK,OBJNAME_DESCRIPTION)
																			VALUES(SEQ_MARS_OBJECT_ID.nextval,OBJ_INDEX.OBJECTNAME,0,NULL);
                                                                            SELECT MAX(OBJECT_NAME_ID)  INTO GETOBJECTNAMEID FROM T_OBJECT_NAMEINFO;
																	END;
																ELSE
																	BEGIN
																		SELECT T_OBJECT_NAMEINFO.OBJECT_NAME_ID INTO GETOBJECTNAMEID 
                                    FROM T_OBJECT_NAMEINFO 
                                    --INNER JOIN T_REGISTED_OBJECT ON T_REGISTED_OBJECT.OBJECT_NAME_ID = T_OBJECT_NAMEINFO.OBJECT_NAME_ID
                                      --AND T_REGISTED_OBJECT.OBJECT_TYPE = OBJ_INDEX.PARENT
                                    WHERE T_OBJECT_NAMEINFO.OBJECT_HAPPY_NAME=OBJ_INDEX.OBJECTNAME
                                      --AND T_REGISTED_OBJECT.APPLICATION_ID = NEWAPPLICATION_ID 
                                      AND ROWNUM = 1
                                      ;


																	END;

																END IF;


																------ IMPORT IN TABLE : T_REGISTED_OBJECT

																IF GETTYPEID != 0 THEN
																	DECLARE
																		ID NUMBER;
																		SUCCESS VARCHAR2(500);
																		AUTO_LOGREPORT NUMBER:=0;
																	BEGIN

                                          UPDATE T_REGISTED_OBJECT
                                          SET QUICK_ACCESS = OBJ_INDEX.QUICKACCESS
                                          WHERE OBJECT_HAPPY_NAME = OBJ_INDEX.OBJECTNAME
                                            AND APPLICATION_ID = NEWAPPLICATION_ID
                                            AND TYPE_ID = GETTYPEID
                                            AND OBJECT_TYPE = GETOBJECTPARENT;



																					INSERT INTO "T_REGISTED_OBJECT"(
																					OBJECT_ID,
																					OBJECT_HAPPY_NAME,
																					APPLICATION_ID,
																					TYPE_ID,
																					QUICK_ACCESS,
																					OBJECT_TYPE,
																					"COMMENT",
																					ENUM_TYPE,
																					OBJECT_NAME_ID,
																					IS_CHECKERROR_OBJ,
																					OBJ_DATA_SRC
																					)
																					SELECT 
                                            SEQ_MARS_OBJECT_ID.nextval,
                                            OBJ_INDEX.OBJECTNAME,
                                            NEWAPPLICATION_ID,
                                            GETTYPEID,
                                            OBJ_INDEX.QUICKACCESS,
                                            GETOBJECTPARENT,
                                            OBJ_INDEX.OBJECTCOMMENT,
                                            OBJ_INDEX.ENUMTYPE,
                                            GETOBJECTNAMEID,
                                            NULL,
                                            NULL 
                                          FROM DUAL
                                          WHERE NOT EXISTS (
                                            SELECT 1
                                            FROM T_REGISTED_OBJECT x
                                            INNER JOIN T_OBJECT_NAMEINFO y ON y.OBJECT_NAME_ID = x.OBJECT_NAME_ID
                                            WHERE y.OBJECT_HAPPY_NAME = OBJ_INDEX.OBJECTNAME
                                              AND x.APPLICATION_ID = NEWAPPLICATION_ID
                                              AND x.TYPE_ID = GETTYPEID
                                              AND x.OBJECT_TYPE = GETOBJECTPARENT
                                          );
                                        SELECT MAX(OBJECT_ID)  INTO ID FROM T_REGISTED_OBJECT;
																					 -- INSERT ENTRY IN LOG TABLE
																					 SELECT MESSAGE INTO SUCCESS FROM TBLMESSAGE WHERE ID=4;
																					 SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

																					 INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON, feedprocessdetailid)
																					 VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,SUCCESS,FULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
																					 IF RESULT IS NULL THEN
																							RESULT:= 'SUCCESS';
																					 ELSE
																							RESULT:= RESULT || '####' || 'SUCCESS';
																					 END IF;
																	END;
																END IF;

														END;
													ELSE
														DECLARE
															ERROR VARCHAR2(500);
															AUTO_LOGREPORT NUMBER:=0;
														BEGIN
															 SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=1;
															 SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

															INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON, feedprocessdetailid)
															VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,FULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
																IF RESULT IS NULL THEN
																	RESULT:= 'ERROR';
																ELSE
																	RESULT:= RESULT || '####' || 'ERROR';
																END IF;
														END;

													END IF;
												END;
										ELSE
												DECLARE
														ERROR VARCHAR2(500);
														AUTO_LOGREPORT NUMBER:=0;
												BEGIN
														SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=2;

														SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

														INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON, feedprocessdetailid)
														VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,FULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
														IF RESULT IS NULL THEN
																	RESULT:= 'ERROR';
																ELSE
																	RESULT:= RESULT || '####' || 'ERROR';
														END IF;
												END;
									   END IF;

								END LOOP;
								--CLOSE FORTBLSTGGUIOBJECT;
							END;	

						END IF;			
------------------------------------------------------ END - CODE FOR IMPORT OBJECT FILE ---------------------------------
---------------------------------------------------------------------------------------------------------------			

----------------------------------------------------- START - CODE FOR IMPORT COMPAREPARAM ---------------------------------
----------------------------------------------------------------------------------------------------------------------------
						IF I_INDEX.FILETYPE = 'COMPAREPARAM' THEN

							BEGIN
								DECLARE

								COMPAREPARAMFULLDATA CLOB;
								GETDATASOURCENAME VARCHAR2(500):=NULL;
								GETDATASOURCETYPEID NUMBER:=0;
								ERROR VARCHAR2(500);
								CURRENTDATE TIMESTAMP;

								CURSOR FORTBLSTGCOMPAREPARAM IS
								SELECT DATASOURCENAME,DATASOURCETYPE,DETAILS FROM TBLSTGCOMPAREPARAM WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

								BEGIN

									FOR COMPARE_INDEX IN FORTBLSTGCOMPAREPARAM
									LOOP

										SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

										COMPAREPARAMFULLDATA := (COMPARE_INDEX.DATASOURCENAME || '####' || COMPARE_INDEX.DATASOURCETYPE  || '####' || COMPARE_INDEX.DETAILS); 
										-- 1 -- COMPARE -- 2 -- CONNECTION  -- 3 -- QUERY -- 4 -- PROFILE

										IF COMPARE_INDEX.DATASOURCETYPE  IN (1,2,3,4) THEN
											DECLARE
												AUTO_LOGREPORT NUMBER:=0;
											BEGIN
														-- CHECK DATASOURCENAME FROM EXCEL IS AVLIABLE THEN GET DATASOURCETYPEID FROM TABLE IF NOT THEN CREATE NEW AND GET THAT

														SELECT COUNT(1) INTO GETDATASOURCETYPEID FROM T_DATA_SOURCE 
                            WHERE T_DATA_SOURCE.DATA_SOURCE_NAME=COMPARE_INDEX.DATASOURCENAME AND ROWNUM = 1;
                            --dbms_output.put_line(GETDATASOURCETYPEID);
                          	IF GETDATASOURCETYPEID=0 THEN
															BEGIN
																-- CREATE NEW ENTRY

																   INSERT INTO T_DATA_SOURCE(DATA_SOURCE_ID,DATA_SOURCE_NAME,DATA_SOURCE_TYPE,DETAILS,DB_CONNECTION,DB_TYPE,TEST_CONNECTION)
																   VALUES(T_TEST_STEPS_SEQ.nextval,COMPARE_INDEX.DATASOURCENAME,COMPARE_INDEX.DATASOURCETYPE, COMPARE_INDEX.DETAILS,NULL,NULL,NULL);
                                                                   SELECT MAX(DATA_SOURCE_ID) INTO GETDATASOURCETYPEID FROM T_DATA_SOURCE;
															END;
														ELSE
															BEGIN
																-- UPDATE ENTRY
                                IF COMPARE_INDEX.DATASOURCENAME = 'SanjayTest'
                                THEN 
                                BEGIN
                                  dbms_output.put_line(COMPARE_INDEX.DATASOURCENAME || ' -- ' || COMPARE_INDEX.DETAILS);
                                END;
                                END IF;
                                  SELECT DATA_SOURCE_ID INTO GETDATASOURCETYPEID FROM T_DATA_SOURCE 
                                  WHERE T_DATA_SOURCE.DATA_SOURCE_NAME=COMPARE_INDEX.DATASOURCENAME AND ROWNUM = 1;

																   UPDATE T_DATA_SOURCE SET 
																   T_DATA_SOURCE.DATA_SOURCE_TYPE=COMPARE_INDEX.DATASOURCETYPE ,
																   T_DATA_SOURCE.DETAILS=COMPARE_INDEX.DETAILS 
																   WHERE  T_DATA_SOURCE.DATA_SOURCE_ID=GETDATASOURCETYPEID; 
															END;
														END IF;												

														---- INSERT LOG  AS SCCESS IN LOGDETAIL TABLE
														SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=4;
														SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

														INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
														VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,COMPAREPARAMFULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
														IF RESULT IS NULL THEN
																	RESULT:= 'SUCCESS';
																ELSE
																	RESULT:= RESULT || '####' || 'SUCCESS';
														END IF;
											END;
										ELSE
											DECLARE
												AUTO_LOGREPORT NUMBER:=0;
											BEGIN
														SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=3;
														SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

														INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
														VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,COMPAREPARAMFULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
														IF RESULT IS NULL THEN
																	RESULT:= 'ERROR';
																ELSE
																	RESULT:= RESULT || '####' || 'ERROR';
														END IF;
											END;
										END IF;
									END LOOP;
									--CLOSE FORTBLSTGCOMPAREPARAM;
								END;

							END;

						END IF;

----------------------------------------------------- END - CODE FOR IMPORT COMPAREPARAM ---------------------------------
--------------------------------------------------------------------------------------------------------------------------

----------------------------------------------------- START - CODE FOR IMPORT TEST CASE ---------------------------------
-------------------------------------------------------------------------------------------------------------------------


						IF I_INDEX.FILETYPE = 'TESTCASE' THEN
							DECLARE
									VALIDATEMESSAGE VARCHAR2(5000);
									TESTSUITECOUNT NUMBER:=0;
									APPLICATIONID NUMBER:=0;
									COUNT_APPLICATIONID NUMBER:=0;
									CURRENTDATE TIMESTAMP;
									TESTSUITENAME VARCHAR(5000);
					                TESTCASENAME VARCHAR(5000);
							BEGIN

                                    -- Remove duplicate staging rows

                                    DELETE FROM tblstgtestcase
                                    WHERE ID IN (
                                        SELECT DISTINCT ID
                                        from tblstgtestcase t
                                        where t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                        AND EXISTS (
                                            SELECT 1
                                            FROM tblstgtestcase s
                                            WHERE s.ID > t.ID
                                                AND s.feedprocessdetailid = t.feedprocessdetailid
                                                AND (
                                                    --t.TESTSUITENAME = s.TESTSUITENAME AND  t.TESTCASENAME = s.TESTCASENAME  AND t.TESTCASEDESCRIPTION = s.TESTCASEDESCRIPTION AND NVL(t.DATASETMODE, 'N/A') = NVL(s.DATASETMODE, 'N/A') AND  t.KEYWORD = s.KEYWORD AND  NVL(t.OBJECT, 'N/A') = NVL(s.OBJECT, 'N/A') AND  NVL(t.PARAMETER, 'N/A') = NVL(s.PARAMETER, 'N/A') AND NVL(t.COMMENTS, 'N/A') = NVL(s.COMMENTS, 'N/A') AND t.DATASETNAME = s.DATASETNAME AND  t.DATASETVALUE = s.DATASETVALUE AND  t.ROWNUMBER = s.ROWNUMBER AND  t.FEEDPROCESSDETAILID = s.FEEDPROCESSDETAILID AND  t.TABNAME = s.TABNAME AND  t.APPLICATION = s.APPLICATION AND  t.CREATEDON = s.CREATEDON
                                                    t.TESTSUITENAME = s.TESTSUITENAME AND  t.TESTCASENAME = s.TESTCASENAME  AND t.TESTCASEDESCRIPTION = s.TESTCASEDESCRIPTION AND NVL(t.DATASETMODE, 'N/A') = NVL(s.DATASETMODE, 'N/A') AND  t.KEYWORD = s.KEYWORD AND  NVL(t.OBJECT, 'N/A') = NVL(s.OBJECT, 'N/A') AND  NVL(t.PARAMETER, 'N/A') = NVL(s.PARAMETER, 'N/A') AND NVL(t.COMMENTS, 'N/A') = NVL(s.COMMENTS, 'N/A') AND t.DATASETNAME = s.DATASETNAME AND  NVL(t.DATASETVALUE, 'N/A') = NVL(s.DATASETVALUE, 'N/A') AND  t.FEEDPROCESSDETAILID = s.FEEDPROCESSDETAILID AND  t.TABNAME = s.TABNAME AND  t.APPLICATION = s.APPLICATION AND s.rownumber = t.rownumber
                                                )
                                        )
                                    );    
                                    commit;    

									SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

									SELECT COUNT(*) INTO COUNT_APPLICATIONID FROM T_REGISTERED_APPS,TBLSTGTESTCASE WHERE  
									T_REGISTERED_APPS.APP_SHORT_NAME= TBLSTGTESTCASE.APPLICATION
									AND TBLSTGTESTCASE.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

								IF (COUNT_APPLICATIONID = 0) THEN
									BEGIN
                                        SELECT count(*) INTO COUNT_APPLICATIONID
                                        FROM (
                                            SELECT Application 
                                            FROM (
                                                    SELECT 
                                                        trim(regexp_substr(Application, '[^,]+', 1, level)) Application
                                                    FROM (
                                                        SELECT DISTINCT Application 
                                                        FROM TBLSTGTESTCASE 
                                                        WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                    ) t
                                                    CONNECT BY instr(Application, ',', 1, level - 1) > 0
                                            ) xy

                                            WHERE EXISTS (
                                                SELECT 1
                                                FROM T_REGISTERED_APPS 
                                                WHERE T_REGISTERED_APPS.APP_SHORT_NAME = xy.Application
                                            )
                                        ) xyz;

									end;
									end if;		    

								IF COUNT_APPLICATIONID!=0 THEN
									DECLARE
										AUTO_TEMP1 NUMBER:=0;
									BEGIN

                                    --dbms_output.put_line('COUNT_APPLICATIONID '||COUNT_APPLICATIONID);
											-- Code developed by Shivam -- Start
                                            					SELECT DISTINCT upper(TBLSTGTESTCASE.TESTSUITENAME) INTO TESTSUITENAME
					                                        FROM TBLSTGTESTCASE
                                            					WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID
                                                                AND ROWNUM = 1;



					                                            -- Code for inserting folse Application in the temp table.

					                                            -- Delete data from temporary table
                                            					DELETE FROM TEMPFALSEAPPLICATION;

                                            					-- Insert the false data
                                            INSERT INTO TEMPFALSEAPPLICATION
                                            (
                                                TESTSUITNAME,
                                                TESTCASENAME,
                                                APPLICATION,
                                                Flag
                                            )
                                            SELECT TESTSUITENAME,x.TESTCASENAME,Application,CASE WHEN EXISTS (SELECT 1 FROM T_REGISTERED_APPS WHERE T_REGISTERED_APPS.APP_SHORT_NAME = x.Application) THEN 1 ELSE 0 END
                                            FROM (
                                                SELECT DISTINCT
                                                    trim(regexp_substr(Application, '[^,]+', 1, level)) Application,testcasename
                                                FROM (
                                                        SELECT DISTINCT Application, tblstgtestcase.testcasename
                                                        FROM TBLSTGTESTCASE 
                                                        WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                     ) t
                                                CONNECT BY instr(Application, ',', 1, level - 1) > 0
                                                order by Application
                                            ) x;
                                            -- 
                                            -- Code developed by Shivam -- End

                                            -- GET TOP 1 Application_ID
                                        SELECT NVL(APPLICATION_ID, 0) INTO APPLICATIONID
                                        FROM T_REGISTERED_APPS
                                        WHERE APP_SHORT_NAME IN (
                                          SELECT APPLICATION
                                          FROM TEMPFALSEAPPLICATION
                                          WHERE ROWNUM = 1
                                        );

                                            delete from TEMP1;
											SELECT  TEMP1_SEQ.NEXTVAL INTO AUTO_TEMP1 FROM DUAL;

											INSERT INTO TEMP1 (TESTCASENAME)
											SELECT TESTSUITENAME FROM TBLSTGTESTCASE  WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID GROUP BY  TESTSUITENAME;

											SELECT COUNT(*) INTO TESTSUITECOUNT FROM TEMP1;
--dbms_output.put_line('TESTSUITECOUNT '||TESTSUITECOUNT);

							IF TESTSUITECOUNT = 1 THEN -- IF - 2 -START
									DECLARE
											AUTO_TEMP1 NUMBER:=0;
											AUTO_TEMPDWTESTCASE NUMBER:=0;  -- UNDER OBSERVATION
											AUTO_TEMPSTGTESTCASE NUMBER:=0; -- UNDER OBSERVATION
											CURRENTDATE TIMESTAMP;

									BEGIN

											--- LOGIC FOR ORDER MATCH -

											SELECT SYSDATE INTO CURRENTDATE FROM DUAL;
											DELETE FROM TEMP1;


											SELECT  TEMPSTGTESTCASE_SEQ.NEXTVAL INTO AUTO_TEMPSTGTESTCASE FROM DUAL;

											INSERT INTO TEMPSTGTESTCASE(TESTCASENAME,DATASETNAME,KEYWORDNAME,OBJECTNAME,ORDERNUMBER) 
											SELECT DISTINCT TESTCASENAME,DATASETNAME,KEYWORD,OBJECT,ROWNUMBER FROM TBLSTGTESTCASE WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;


											SELECT  TEMPDWTESTCASE_SEQ.NEXTVAL INTO AUTO_TEMPDWTESTCASE FROM DUAL;

                                            INSERT INTO TEMPDWTESTCASE(TESTCASENAME,DATASETNAME,KEYWORDNAME,OBJECTNAME,ORDERNUMBER) 
                                            SELECT DISTINCT
                                                T_TEST_CASE_SUMMARY.TEST_CASE_NAME,
                                                DATASETNAME,T_KEYWORD.KEY_WORD_NAME,TBLSTGTESTCASE.OBJECT,TBLSTGTESTCASE.ROWNUMBER 
                                            FROM TBLSTGTESTCASE
                                            INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                            INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME)  =UPPER(TBLSTGTESTCASE.KEYWORD)
                                            WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

											/*
                                            INSERT INTO TEMPDWTESTCASE(TESTCASENAME,DATASETNAME,KEYWORDNAME,OBJECTNAME,ORDERNUMBER) 
											SELECT 
											T_TEST_CASE_SUMMARY.TEST_CASE_NAME,
											DATASETNAME,T_KEYWORD.KEY_WORD_NAME,T_REGISTED_OBJECT.OBJECT_HAPPY_NAME,TBLSTGTESTCASE.ROWNUMBER 
                                            FROM T_TEST_CASE_SUMMARY ,T_KEYWORD,T_REGISTED_OBJECT,TBLSTGTESTCASE
											WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(TBLSTGTESTCASE.TESTCASENAME)
											AND UPPER(T_KEYWORD.KEY_WORD_NAME)  =UPPER(TBLSTGTESTCASE.KEYWORD)
											--AND UPPER(T_REGISTED_OBJECT.OBJECT_HAPPY_NAME)=UPPER(TBLSTGTESTCASE.OBJECT)
											AND TBLSTGTESTCASE.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID GROUP BY T_TEST_CASE_SUMMARY.TEST_CASE_NAME,
											DATASETNAME,T_KEYWORD.KEY_WORD_NAME,T_REGISTED_OBJECT.OBJECT_HAPPY_NAME,TBLSTGTESTCASE.ROWNUMBER
											ORDER BY TBLSTGTESTCASE.ROWNUMBER;
                                            */



											SELECT  TEMP1_SEQ.NEXTVAL INTO AUTO_TEMP1 FROM DUAL;

											INSERT INTO TEMP1 (TESTCASENAME,DATASETNAME)
											SELECT DISTINCT TEMPSTGTESTCASE.TESTCASENAME,TEMPSTGTESTCASE.DATASETNAME
											FROM TEMPSTGTESTCASE
											WHERE NOT EXISTS(SELECT TEMPDWTESTCASE.TESTCASENAME,TEMPDWTESTCASE.DATASETNAME 
											FROM TEMPDWTESTCASE 
											WHERE UPPER(TEMPDWTESTCASE.TESTCASENAME) =UPPER(TEMPSTGTESTCASE.TESTCASENAME)
											AND UPPER(TEMPDWTESTCASE.DATASETNAME)=UPPER(TEMPSTGTESTCASE.DATASETNAME)
											AND UPPER(TEMPDWTESTCASE.KEYWORDNAME)=UPPER(TEMPSTGTESTCASE.KEYWORDNAME)
											--AND UPPER(TEMPDWTESTCASE.OBJECTNAME)=UPPER(TEMPSTGTESTCASE.OBJECTNAME)
											AND TEMPDWTESTCASE.ORDERNUMBER = TEMPSTGTESTCASE.ORDERNUMBER
											);



----------------------------------------------------------------------------------------------------------------------------------------
                            --dbms_output.put_line('Overwrite Value: ' || ISOVERWRITE);
                        /*    
                    	IF ISOVERWRITE != 0 THEN  -- 0 MEANS FALSE ELSE TRUE LIKE 1 -- IF - 3 -START

----------------------------------------------------------- DELETE FROM DATAWARE HOUSE CODE - START --------------------------------------------------------------------					
                                        -- Performance code by Shivam Parikh - Start
                                        DECLARE
                                            ERROR varchar2(5000);
                                            COUNTOFRECORDS INT;
                                        BEGIN  

                                            -- Addd Log of all delete records from warehouse - Start
                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=20;

                                                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,LOGDETAIL,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                                                FROM (    
                                                    SELECT DISTINCT 'TESTCASE NAME :' || tblstgtestcase.TESTCASENAME  || ' : ' || tblstgtestcase.DATASETNAME AS LOGDETAIL
                                                    FROM tblstgtestcase
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                ) xyz;

                                                -- Add log for Success Found - Start
                                                SELECT count(1) INTO COUNTOFRECORDS
                                                FROM tblstgtestcase
                                                WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                    AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                    AND EXISTS (
                                                        SELECT 1
                                                        FROM T_TEST_CASE_SUMMARY 
                                                        WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    );
                                                IF (COUNTOFRECORDS > 0) 
                                                THEN    
                                                    BEGIN
                                                        IF RESULT IS NULL THEN
                                                                    RESULT:= 'SUCCESS';
                                                                ELSE
                                                                    RESULT:= RESULT || '####' || 'SUCCESS';
                                                        END IF;
                                                    END;
                                                END IF;
                                                -- Add log for Success Found - End
                                            -- Addd Log of all delete records from warehouse - Start


                                            -- Add log for Test Case Not Found - Start
                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=19;

                                                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,LOGDETAIL,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                                                FROM (    
                                                    SELECT DISTINCT tblstgtestcase.TESTCASENAME AS LOGDETAIL
                                                    FROM tblstgtestcase
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM T_TEST_CASE_SUMMARY 
                                                            WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                        )
                                                ) xyz;

                                                -- Append RESULT for Error

                                                SELECT COUNT(1) INTO COUNTOFRECORDS
                                                FROM tblstgtestcase
                                                WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                    AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM T_TEST_CASE_SUMMARY 
                                                        WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    );
                                                IF (COUNTOFRECORDS > 0)
                                                THEN
                                                    BEGIN
                                                        IF RESULT IS NULL THEN
                                                                    RESULT:= 'ERROR';
                                                                ELSE
                                                                    RESULT:= RESULT || '####' || 'ERROR';
                                                        END IF;
                                                    END;
                                                END IF;
                                                -- Append RESULT for Error
                                            -- Add log for Test Case Not Found - End

                                            -- Data Set Delete code -- Start

                                                dbms_output.put_line('delete started');
                                                DELETE FROM REL_TC_DATA_SUMMARY 
                                                WHERE REL_TC_DATA_SUMMARY.DATA_SUMMARY_ID IN (
                                                    SELECT DISTINCT REL_TC_DATA_SUMMARY.DATA_SUMMARY_ID
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    INNER JOIN REL_TC_DATA_SUMMARY ON REL_TC_DATA_SUMMARY.TEST_CASE_ID = T_TEST_CASE_SUMMARY.TEST_CASE_ID
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Data Set Delete code -- End

                                            -- Data Set Value delete code - Start
                                                DELETE FROM TEST_DATA_SETTING 
                                                WHERE TEST_DATA_SETTING.STEPS_ID IN 
                                                (
                                                    SELECT T_TEST_STEPS.STEPS_ID 
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.TEST_CASE_ID = T_TEST_CASE_SUMMARY.TEST_CASE_ID
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Data Set Value delete code - Start

                                            -- Delete from T_TEST_STEPS - Start
                                                DELETE FROM T_TEST_STEPS 
                                                WHERE T_TEST_STEPS.TEST_CASE_ID IN (
                                                    SELECT T_TEST_CASE_SUMMARY.TEST_CASE_ID
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Delete from T_TEST_STEPS - Start

                                            -- Delete from REL_APP_TESTCASE - Start
                                                DELETE FROM REL_APP_TESTCASE 
                                                WHERE REL_APP_TESTCASE.TEST_CASE_ID IN (
                                                    SELECT T_TEST_CASE_SUMMARY.TEST_CASE_ID
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Delete from REL_APP_TESTCASE - End

                                            -- Delete from REL_TEST_CASE_TEST_SUITE - Start

                                            -- Delete from REL_TEST_CASE_TEST_SUITE - Start
                                                DELETE FROM REL_TEST_CASE_TEST_SUITE
                                                WHERE rel_test_case_test_suite.relationship_id IN (
                                                    SELECT DISTINCT rel_test_case_test_suite.relationship_id
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    INNER JOIN t_test_suite ON UPPER(t_test_suite.test_suite_name) = UPPER(tblstgtestcase.testsuitename)
                                                    INNER JOIN rel_test_case_test_suite ON rel_test_case_test_suite.test_case_id = T_TEST_CASE_SUMMARY.test_case_id
                                                        AND rel_test_case_test_suite.test_suite_id = t_test_suite.test_suite_id
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Delete from REL_TEST_CASE_TEST_SUITE - End    

                                            -- Delete Test Case - Start
                                                DELETE FROM T_TEST_CASE_SUMMARY
                                                WHERE T_TEST_CASE_SUMMARY.TEST_CASE_ID IN (
                                                    SELECT DISTINCT T_TEST_CASE_SUMMARY.TEST_CASE_ID
                                                    FROM TBLSTGTESTCASE
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) = UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND TBLSTGTESTCASE.DATASETMODE IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                        )
                                                );
                                            -- Delete Test Case - End

                                        END;   
                                        dbms_output.put_line('delete ended');
                                        -- Performance code by Shivam Parikh - Start

---------------------------------------------------------------------------------------------------------- DELETE FROM DATAWARE HOUSE CODE - END --------------------------------------------------------------------


---------------------------------------------------------------------------------------------------------- LOGIC FOR OVERWRITE - START ----------------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------------- LOGIC FOR OVERWRITE - END ----------------------------------------------------------------------------------------------------------
                            END IF;  
                        */    
---------------------------------------------------------------------------------------------------------- LOGIC FOR NOT OVERWRITE - START ----------------------------------------------------------------------------------------------------------

								--BEGIN -- LOGIC FOR NOT OVERWRITE - START

								  --- CODE FOR IF OVERWITE CONDITION IS FALSE

                            -- Performance Code -- Shivam  - Start
                                DECLARE 
                                    MAXRow INT;
                                    OBJECT_ID INT;
                                    OBJECT_ID_Set INT;
                                    AUTO_LOGREPORT INT;
                                    ERROR VARCHAR2(500);
                                    MESSAGE VARCHAR2(500);
                                    CURRENTTIME TIMESTAMP;
                                    COUNT_T_TEST_STEPS INT:=0;
                                    COUNT_T_SHARED_OBJECT_POOL INT:=0;
                                    COUNT_TEST_DATA_SETTING INT:=0;
                                    COUNT_DATA_SET_DESCRIPTION INT:=0;
                                BEGIN    
                                    --dbms_output.put_line('Log Insert Started');
                                    SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                    --dbms_output.put_line(CURRENTTIME);
                                -- Insert All Log Reports - Start  
                                    -- Insert log for the application not found for the tab.
                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=18;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,STATUS,LOGDETAILS, (LOGREPORT_SEQ.NEXTVAL),(SELECT SYSDATE FROM DUAL) AS CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT 'DATASET : '|| tblstgtestcase.DATASETNAME || ERROR || '-' || tblstgtestcase.TABNAME AS STATUS, tblstgtestcase.TESTSUITENAME || '####' ||tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.TESTCASEDESCRIPTION  || '####' || tblstgtestcase.DATASETMODE  || '####' || tblstgtestcase.KEYWORD  || '####' || tblstgtestcase.OBJECT  || '####' || tblstgtestcase.PARAMETER  || '####' || tblstgtestcase.COMMENTS  || '####' || tblstgtestcase.DATASETNAME  || '####' || tblstgtestcase.DATASETVALUE  || '####' || tblstgtestcase.ROWNUMBER  || '####' || tblstgtestcase.FEEDPROCESSDETAILID || '####' || tblstgtestcase.TABNAME LOGDETAILS
                                            FROM tblstgtestcase
                                            INNER JOIN TEMPFALSEAPPLICATION ON TEMPFALSEAPPLICATION.TESTSUITNAME = tblstgtestcase.testsuitename
                                                AND TEMPFALSEAPPLICATION.TESTCASENAME = tblstgtestcase.testcasename
                                                AND tempfalseapplication.flag = 0
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                        ) x;    

                                        IF RESULT IS NULL THEN
                                                    RESULT:= 'ERROR';
                                                ELSE
                                                    RESULT:= RESULT || '####' || 'ERROR';
                                        END IF;
                                    -- Insert log for the application not found for the tab.

                                    -- Inser log for succcess new insert records - Start

                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=4;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,STATUS,LOGDETAILS,LOGREPORT_SEQ.NEXTVAL,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT 'DATASET : '|| tblstgtestcase.DATASETNAME || ERROR || '-' || tblstgtestcase.TABNAME AS STATUS,tblstgtestcase.TESTSUITENAME || '####' || tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.TESTCASEDESCRIPTION  || '####' || tblstgtestcase.DATASETMODE  || '####' || tblstgtestcase.KEYWORD  || '####' || tblstgtestcase.OBJECT  || '####' || tblstgtestcase.PARAMETER  || '####' || tblstgtestcase.COMMENTS  || '####' || tblstgtestcase.DATASETNAME  || '####' || tblstgtestcase.DATASETVALUE  || '####' || tblstgtestcase.ROWNUMBER  || '####' || tblstgtestcase.FEEDPROCESSDETAILID || '####' || tblstgtestcase.TABNAME AS LOGDETAILS
                                            FROM tblstgtestcase
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM tempfalseapplication
                                                    where tempfalseapplication.testcasename = TBLSTGTESTCASE.TESTCASENAME
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                        );        
                                        IF RESULT IS NULL THEN
                                                    RESULT:= 'SUCCESS';
                                                ELSE
                                                    RESULT:= RESULT || '####' || 'SUCCESS';
                                        END IF;
                                    -- Inser log for succcess new insert/Update records - Start


                                    -- Inser log for Keyword not found - Start
                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=1;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,LOGDETAILS,LOGREPORT_SEQ.NEXTVAL,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT tblstgtestcase.TESTSUITENAME || '####' || tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.TESTCASEDESCRIPTION  || '####' || tblstgtestcase.DATASETMODE  || '####' || tblstgtestcase.KEYWORD  || '####' || tblstgtestcase.OBJECT  || '####' || tblstgtestcase.PARAMETER  || '####' || tblstgtestcase.COMMENTS  || '####' || tblstgtestcase.DATASETNAME  || '####' || tblstgtestcase.DATASETVALUE  || '####' || tblstgtestcase.ROWNUMBER  || '####' || tblstgtestcase.FEEDPROCESSDETAILID || '####' || tblstgtestcase.TABNAME LOGDETAILS
                                            FROM tblstgtestcase
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                             and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM t_keyword 
                                                    WHERE UPPER(t_keyword.key_word_name) = UPPER(tblstgtestcase.KEYWORD)

                                                    AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM tempfalseapplication
                                                    where tempfalseapplication.testcasename = TBLSTGTESTCASE.TESTCASENAME
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                        );        
                                        IF RESULT IS NULL THEN
                                                    RESULT:= 'SUCCESS';
                                                ELSE
                                                    RESULT:= RESULT || '####' || 'SUCCESS';
                                        END IF;
                                    -- Inser log for Keyword not found - End

                                    -- Insert log for count of dataset in spreadsheet and count of datasets in warehouse does not match - start

                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=7;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,feedprocessdetailid)
                                        --VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,'DATASET : '|| TESTCASE_INDEX.DATASETNAME || ERROR || '-' || TESTCASE_INDEX.TABNAME,TESTCASEFULLDATA,AUTO_LOGREPORT,CURRENTDATE);
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE, STATUS, '' AS LOGDETAILS, (LOGREPORT_SEQ.NEXTVAL), (SELECT SYSDATE FROM DUAL) AS CURRENTDATE ,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT 'DATASET : '|| x.TESTCASENAME || ERROR || '-' || x.DATASETNAME as STATUS
                                            FROM (
                                                SELECT TESTCASENAME,DATASETNAME,COUNT(1) as countofsteps
                                                FROM TBLSTGTESTCASE
                                                WHERE KEYWORD IS NOT NULL 
                                                    AND FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                    AND DATASETMODE IS NULL
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM TEMPFALSEAPPLICATION
                                                        WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                            AND tempfalseapplication.FLAG = 0
                                                            AND ROWNUM=1
                                                    )
                                                GROUP BY TESTCASENAME,DATASETNAME
                                            ) x    
                                            INNER JOIN 
                                            (
                                                SELECT tblstgtestcase.TESTCASENAME, tblstgtestcase.DATASETNAME, COUNT(1) as countofsteps
                                                FROM tblstgtestcase
                                                INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.TESTCASENAME)
                                                INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.DATASETNAME)
                                                INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgtestcase.KEYWORD)
                                                INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.TEST_CASE_ID
                                                    AND T_TEST_STEPS.KEY_WORD_ID = T_KEYWORD.KEY_WORD_ID
                                                    AND T_TEST_STEPS.RUN_ORDER = tblstgtestcase.ROWNUMBER
                                                    AND T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                WHERE tblstgtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                and  tblstgtestcase.KEYWORD IS NOT NULL 
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM TEMPFALSEAPPLICATION
                                                        WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                            AND tempfalseapplication.FLAG = 0
                                                            AND ROWNUM=1
                                                    )
                                                GROUP BY tblstgtestcase.TESTCASENAME, tblstgtestcase.DATASETNAME
                                                ORDER BY 2
                                            ) y
                                            ON x.TESTCASENAME = y.TESTCASENAME
                                                AND x.DATASETNAME = y.DATASETNAME
                                                AND x.countofsteps <> y.countofsteps     
                                        ) xyz;        
                                    -- Insert log for count of dataset in spreadsheet and count of datasets in warehouse does not match - end

                                    -- Insert log for Order mathced but Step ID not found - Start
                                    --=== NEED TO DISCUSS THE BEHAVIOR    
                                    -- Insert log for Order mathced but Step ID not found - End

                                    -- Insert log for records where Order of test case not matched - start

                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=13;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,STATUS,LOGDETAILS,LOGREPORT_SEQ.NEXTVAL, (SELECT SYSDATE FROM DUAL) AS CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT FEEDPROCESSDETAILID,'DATASET : '|| tblstgtestcase.DATASETNAME || ERROR || '-' || tblstgtestcase.TABNAME AS STATUS,tblstgtestcase.TESTSUITENAME || '####' || tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.TESTCASEDESCRIPTION  || '####' || tblstgtestcase.DATASETMODE  || '####' || tblstgtestcase.KEYWORD  || '####' || tblstgtestcase.OBJECT  || '####' || tblstgtestcase.PARAMETER  || '####' || tblstgtestcase.COMMENTS  || '####' || tblstgtestcase.DATASETNAME  || '####' || tblstgtestcase.DATASETVALUE  || '####' || tblstgtestcase.ROWNUMBER  || '####' || tblstgtestcase.FEEDPROCESSDETAILID || '####' || tblstgtestcase.TABNAME AS LOGDETAILS
                                            FROM tblstgtestcase
                                            INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                                AND tblstgtestcase.datasetname IS NOT NULL
                                            INNER JOIN T_KEYWORD ON T_KEYWORD.KEY_WORD_NAME = tblstgtestcase.KEYWORD
                                            CROSS JOIN TABLE(GETTOPOBJECT(tblstgtestcase.object, APPLICATIONID)) t
                                            WHERE tblstgtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.datasetvalue IS NOT NULL
                                                AND tblstgtestcase.datasetmode IS NULL
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM t_test_steps
                                                    WHERE t_test_steps.key_word_id = t_keyword.key_word_id
                                                        AND t_test_steps.object_id = t.OBJECT_ID
                                                        AND t_test_steps.test_case_id = T_TEST_CASE_SUMMARY.test_case_id
                                                        AND t_test_steps.run_order = tblstgtestcase.rownumber
                                                        AND ROWNUM=1
                                                )
                                        );
                                    -- Insert log for records where Order of test case not matched - end

                                    -- Insert log for records where Order of test case not matched - start
                                        SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=8;

                                        INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                                        SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,STATUS,LOGDETAILS,LOGREPORT_SEQ.NEXTVAL, (SELECT SYSDATE FROM DUAL) AS CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT FEEDPROCESSDETAILID,'DATASET : '|| tblstgtestcase.DATASETNAME || ERROR || '-' || tblstgtestcase.TABNAME AS STATUS,tblstgtestcase.TESTSUITENAME || '####' || tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.TESTCASEDESCRIPTION  || '####' || tblstgtestcase.DATASETMODE  || '####' || tblstgtestcase.KEYWORD  || '####' || tblstgtestcase.OBJECT  || '####' || tblstgtestcase.PARAMETER  || '####' || tblstgtestcase.COMMENTS  || '####' || tblstgtestcase.DATASETNAME  || '####' || tblstgtestcase.DATASETVALUE  || '####' || tblstgtestcase.ROWNUMBER  || '####' || tblstgtestcase.FEEDPROCESSDETAILID || '####' || tblstgtestcase.TABNAME AS LOGDETAILS
                                            FROM tblstgtestcase
                                            INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                                AND tblstgtestcase.datasetname IS NOT NULL
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                            INNER JOIN T_KEYWORD ON T_KEYWORD.KEY_WORD_NAME = tblstgtestcase.KEYWORD
                                            CROSS JOIN TABLE(GETTOPOBJECT(tblstgtestcase.object, APPLICATIONID)) t
                                            WHERE tblstgtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.datasetvalue IS NOT NULL
                                                AND tblstgtestcase.datasetmode IS NULL
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                                 AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM t_test_steps
                                                    WHERE t_test_steps.key_word_id = t_keyword.key_word_id
                                                        AND t_test_steps.object_id = t.OBJECT_ID
                                                        AND t_test_steps.test_case_id = T_TEST_CASE_SUMMARY.test_case_id
                                                        AND ROWNUM=1
                                                        --AND t_test_steps.run_order = tblstgtestcase.rownumber
                                                )
                                        );
                                    -- Insert log for records where Order of test case not matched - end

                                    -- Insert log for Skip Mode - start
                                        INSERT INTO "LOGREPORT" (FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                                        SELECT tbl.filename,tbl.filetype,'DATASETMODE:-X AND DATASET : ' || tbl.DATASETNAME || msg.MESSAGE AS Status,'' AS LogDetails,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                                        FROM (
                                            SELECT DISTINCT fd.filename,fd.filetype,stg.datasetname,'' aS LOGDETAILS 
                                            FROM TBLSTGTESTCASE stg
                                            INNER JOIN tblfeedprocessdetails fd ON fd.feedprocessdetailid = stg.feedprocessdetailid
                                            WHERE stg.FEEDPROCESSDETAILID= I_INDEX.FEEDPROCESSDETAILID 
                                                AND stg.DATASETMODE IS NOT NULL  
                                                 and stg.KEYWORD IS NOT NULL 
                                                AND stg.DATASETMODE='X'
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = stg.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                        ) tbl,TBLMESSAGE msg
                                        WHERE msg.ID = 9;
                                    -- Insert log for Skip Mode - end
                                -- Insert All Log Reports - End
                                --dbms_output.put_line('Log Insert Ended');
                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                --dbms_output.put_line(CURRENTTIME);

                                --dbms_output.put_line('All Update Started');
                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                --dbms_output.put_line(CURRENTTIME);
                                -- All update Codes - start

                                -- Update T_TEST_STEPS - Start
                                SELECT COUNT(*) INTO COUNT_T_TEST_STEPS
                                            FROM tblstgtestcase stg
                                            INNER JOIN t_test_case_summary tcs ON UPPER(tcs.test_case_name) = UPPER(stg.testcasename)
                                            INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                            INNER JOIN T_TEST_STEPS tts ON tts.KEY_WORD_ID = tk.KEY_WORD_ID
                                                AND tts.RUN_ORDER = stg.ROWNUMBER
                                                AND tts.TEST_CASE_ID = tcs.test_case_id
                                            CROSS JOIN TABLE(GETTOPOBJECT(stg.object, APPLICATIONID)) t
                                            WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND stg.DATASETMODE IS NULL
                                                 and stg.KEYWORD IS NOT NULL ;
                                IF COUNT_T_TEST_STEPS !=0 THEN

                                     MERGE INTO T_TEST_STEPS TTS1
                                        USING (
                                            SELECT DISTINCT tts.STEPS_ID,t.OBJECT_ID, tk.KEY_WORD_ID,t.OBJECT_NAME_ID, stg.parameter, stg.COMMENTS
                                            FROM tblstgtestcase stg
                                            INNER JOIN t_test_case_summary tcs ON UPPER(tcs.test_case_name) = UPPER(stg.testcasename)
                                            INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                            INNER JOIN T_TEST_STEPS tts ON tts.KEY_WORD_ID = tk.KEY_WORD_ID
                                                AND tts.RUN_ORDER = stg.ROWNUMBER
                                                AND tts.TEST_CASE_ID = tcs.test_case_id
                                            CROSS JOIN TABLE(GETTOPOBJECT(stg.object, APPLICATIONID)) t
                                            WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND stg.DATASETMODE IS NULL
                                                 and stg.KEYWORD IS NOT NULL 
                                        ) TMP
                                        ON (TTS1.STEPS_ID = TMP.STEPS_ID)
                                        WHEN MATCHED THEN
                                        UPDATE SET
                                            --TTS1.KEY_WORD_ID = TMP.KEY_WORD_ID,
                                            TTS1.OBJECT_ID = TMP.OBJECT_ID,
                                            TTS1.COLUMN_ROW_SETTING = TMP.parameter,
                                            TTS1.OBJECT_NAME_ID = TMP.OBJECT_NAME_ID,
                                            TTS1."COMMENT" = TMP.COMMENTS;
                                    END IF;
                                -- Update T_TEST_STEPS - End

                                -- Update T_SHARED_OBJECT_POOL - Start
                                SELECT COUNT(*) INTO COUNT_T_SHARED_OBJECT_POOL
                                            FROM tblstgtestcase stg
                                            INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                            INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                                AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                                AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                            WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND stg.DATASETMODE IS NULL
                                                and   stg.KEYWORD IS NOT NULL ;
                                IF COUNT_T_SHARED_OBJECT_POOL !=0 THEN

                                        DECLARE
                                          OBJPOOLidu INT:=0;
                                          DATASETVALUEU VARCHAR2(1000):='';
                                          CURSOR TSHAREDOBJ
                                          IS
                                          SELECT DISTINCT tsop.OBJECT_POOL_ID,stg.DATASETVALUE
                                          FROM tblstgtestcase stg
                                          INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                          INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                              AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                              AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                          WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                              AND stg.DATASETMODE IS NULL
                                               and stg.KEYWORD IS NOT NULL;
                                          BEGIN

                                            FOR TSHAREDOBJ_INDEX IN TSHAREDOBJ
                                            LOOP

                                              UPDATE T_SHARED_OBJECT_POOL
                                              SET DATA_VALUE = TSHAREDOBJ_INDEX.DATASETVALUE
                                              WHERE OBJECT_POOL_ID = TSHAREDOBJ_INDEX.OBJECT_POOL_ID;

                                            END LOOP;

                                          END;
                                          /*
                                         MERGE INTO T_SHARED_OBJECT_POOL tsop1
                                            USING (
                                                SELECT DISTINCT tsop.OBJECT_POOL_ID,stg.DATASETVALUE
                                                FROM tblstgtestcase stg
                                                INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                                INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                                    AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                                    AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                                WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                    AND stg.DATASETMODE IS NULL
                                                     and stg.KEYWORD IS NOT NULL 
                                            ) TMP
                                            ON (tsop1.OBJECT_POOL_ID = TMP.OBJECT_POOL_ID)
                                            WHEN MATCHED THEN
                                            UPDATE SET
                                                tsop1.DATA_VALUE = TMP.DATASETVALUE;
                                         */
                                        END IF;
                                -- Update T_SHARED_OBJECT_POOL - Start

                                -- Update TEST_DATA_SETTING - Start
                                SELECT COUNT(*) INTO COUNT_TEST_DATA_SETTING
                                            FROM tblstgtestcase stg
                                            INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                            INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                                AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                                AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                            INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                            INNER JOIN t_test_case_summary tcs ON UPPER(tcs.test_case_name) = UPPER(stg.testcasename)
                                            INNER JOIN T_TEST_STEPS tts ON tts.KEY_WORD_ID = tk.KEY_WORD_ID
                                                AND tts.RUN_ORDER = stg.ROWNUMBER
                                                AND tts.TEST_CASE_ID = tcs.test_case_id    
                                            INNER JOIN TEST_DATA_SETTING tdss ON tdss.STEPS_ID = tts.STEPS_ID
                                                AND tdss.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                            WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND stg.DATASETMODE IS NULL
                                                 and stg.KEYWORD IS NOT NULL ;
                                IF COUNT_TEST_DATA_SETTING !=0 THEN
                                  --dbms_output.put_line('@@@@@@@@@@');
                                    --dbms_output.put_line(COUNT_TEST_DATA_SETTING);   
                                    BEGIN
                                      /*
                                      MERGE INTO TEST_DATA_SETTING tdss1
                                        USING (
                                            SELECT DISTINCT tdss.DATA_SETTING_ID,stg.DATASETVALUE, tsop.OBJECT_POOL_ID,stg.skip
                                            FROM tblstgtestcase stg
                                            INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                            INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                                AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                                AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                            INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                            INNER JOIN t_test_case_summary tcs ON UPPER(tcs.test_case_name) = UPPER(stg.testcasename)
                                            INNER JOIN T_TEST_STEPS tts ON tts.KEY_WORD_ID = tk.KEY_WORD_ID
                                                AND tts.RUN_ORDER = stg.ROWNUMBER
                                                AND tts.TEST_CASE_ID = tcs.test_case_id    
                                            INNER JOIN TEST_DATA_SETTING tdss ON tdss.STEPS_ID = tts.STEPS_ID
                                                AND tdss.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                            WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND stg.DATASETMODE IS NULL
                                                 and stg.KEYWORD IS NOT NULL 
                                        ) TMP
                                        ON (tdss1.DATA_SETTING_ID = TMP.DATA_SETTING_ID)
                                        WHEN MATCHED THEN
                                        UPDATE SET
                                            tdss1.DATA_VALUE = TMP.DATASETVALUE,
                                            tdss1.POOL_ID = TMP.OBJECT_POOL_ID,
                                            tdss1.DATA_DIRECTION=TMP.skip ;
                                        */
                                        DECLARE
                                          CURSOR DATASETTINGCURSOR IS 
                                          SELECT DISTINCT tdss.DATA_SETTING_ID,tdss.DATA_VALUE,stg.DATASETVALUE, tdss.POOL_ID,tsop.OBJECT_POOL_ID,tdss.DATA_DIRECTION,stg.skip
                                          FROM tblstgtestcase stg
                                          INNER JOIN t_test_data_summary tds ON UPPER(tds.alias_name) = UPPER(stg.DATASETNAME)
                                          INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                              AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                              AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                          INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                          INNER JOIN t_test_case_summary tcs ON UPPER(tcs.test_case_name) = UPPER(stg.testcasename)
                                          INNER JOIN T_TEST_STEPS tts ON tts.KEY_WORD_ID = tk.KEY_WORD_ID
                                              AND tts.RUN_ORDER = stg.ROWNUMBER
                                              AND tts.TEST_CASE_ID = tcs.test_case_id    
                                          INNER JOIN TEST_DATA_SETTING tdss ON tdss.STEPS_ID = tts.STEPS_ID
                                              AND tdss.DATA_SUMMARY_ID = tds.DATA_SUMMARY_ID
                                          WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                              AND stg.DATASETMODE IS NULL
                                               and stg.KEYWORD IS NOT NULL;
                                      BEGIN
                                        FOR DATASETTIGN_INDEX IN DATASETTINGCURSOR
                                        LOOP
                                           UPDATE TEST_DATA_SETTING
                                            SET DATA_VALUE = DATASETTIGN_INDEX.DATASETVALUE,
                                              POOL_ID = DATASETTIGN_INDEX.OBJECT_POOL_ID,
                                              DATA_DIRECTION=DATASETTIGN_INDEX.skip
                                            WHERE DATA_SETTING_ID = DATASETTIGN_INDEX.DATA_SETTING_ID;
                                        END LOOP;    
                                      END;
                                    END;
                                    END IF;
                                -- Update TEST_DATA_SETTING - End

                                -- UPDATE t_test_data_summary - START

                                SELECT COUNT(*) INTO COUNT_DATA_SET_DESCRIPTION
                                FROM tblstgtestcase stg
                                INNER JOIN T_TEST_CASE_SUMMARY ttcs ON ttcs.TEST_CASE_NAME = stg.TESTCASENAME
                                INNER JOIN REL_TC_DATA_SUMMARY rel ON rel.TEST_CASE_ID = ttcs.TEST_CASE_ID
                                INNER JOIN t_test_data_summary tcs ON UPPER(tcs.ALIAS_NAME) = UPPER(stg.testcasename)
                                  AND tcs.DATA_SUMMARY_ID = rel.DATA_SUMMARY_ID
                                INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

                                IF COUNT_TEST_DATA_SETTING !=0 THEN
                                BEGIN
                                  MERGE INTO t_test_data_summary TTS1
                                  USING (
                                      SELECT DISTINCT tcs.DATA_SUMMARY_ID, stg.DATASETDESCRIPTION
                                      FROM tblstgtestcase stg
                                      INNER JOIN T_TEST_CASE_SUMMARY ttcs ON ttcs.TEST_CASE_NAME = stg.TESTCASENAME
                                      INNER JOIN REL_TC_DATA_SUMMARY rel ON rel.TEST_CASE_ID = ttcs.TEST_CASE_ID
                                      INNER JOIN t_test_data_summary tcs ON UPPER(tcs.ALIAS_NAME) = UPPER(stg.datasetname)
                                        AND tcs.DATA_SUMMARY_ID = rel.DATA_SUMMARY_ID
                                      WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                  ) TMP
                                  ON (TTS1.DATA_SUMMARY_ID = TMP.DATA_SUMMARY_ID)
                                  WHEN MATCHED THEN
                                  UPDATE SET
                                    TTS1.DESCRIPTION_INFO = tmp.DATASETDESCRIPTION;
                                END;
                                END IF;
                                -- UPDATE t_test_data_summary -END
                                -- All update Code ends
                                --dbms_output.put_line('All Update Ended');
                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                --dbms_output.put_line(CURRENTTIME);

                                --dbms_output.put_line('All Insert Started');
                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                --dbms_output.put_line(CURRENTTIME);
                                    -- Insert for new Test Suite -- Start
                                    --CHERISH
                                        --SELECT MAX(TEST_SUITE_ID) INTO MAXRow FROM T_TEST_SUITE;
                                        --remove distinct 
                                        SELECT T_TEST_SUITE_SEQ.nextval INTO MAXRow FROM DUAL;
                                        INSERT INTO T_TEST_SUITE (TEST_SUITE_ID,TEST_SUITE_NAME,TEST_SUITE_DESCRIPTION)
                                        SELECT   T_TEST_SUITE_SEQ.nextval ,TESTSUITENAME,Description
                                        FROM (
                                                SELECT DISTINCT tblstgtestcase.TESTSUITENAME,tblstgtestcase.TESTSUITENAME as Description
                                                FROM tblstgtestcase
                                                WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                    AND tblstgtestcase.DATASETMODE IS NULL 
                                                    and tblstgtestcase.KEYWORD IS NOT NULL 
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM T_TEST_SUITE
                                                        WHERE T_TEST_SUITE.TEST_SUITE_NAME = tblstgtestcase.TESTSUITENAME
                                                        AND ROWNUM=1
                                                    )
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM TEMPFALSEAPPLICATION
                                                        WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                            AND tempfalseapplication.FLAG = 0
                                                            AND ROWNUM=1
                                                    )
                                            );    
                                    -- Insert for new Test Suite -- END

                                    -- Insert for Test Suit Project Relation - start
                                    /*
                                        SELECT MAX(RELATIONSHIP_ID) INTO MAXRow FROM REL_TEST_SUIT_PROJECT;

                                        INSERT INTO REL_TEST_SUIT_PROJECT(RELATIONSHIP_ID,TEST_SUITE_ID,PROJECT_ID)
                                        SELECT DISTINCT MAXRow + ROWNUM AS ID,TEST_SUITE_ID,587
                                        FROM (
                                            SELECT DISTINCT T_TEST_SUITE.TEST_SUITE_ID
                                            FROM tblstgtestcase
                                            INNER JOIN T_TEST_SUITE ON T_TEST_SUITE.TEST_SUITE_NAME = tblstgtestcase.TESTSUITENAME
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM REL_TEST_SUIT_PROJECT
                                                    WHERE REL_TEST_SUIT_PROJECT.TEST_SUITE_ID = T_TEST_SUITE.TEST_SUITE_ID
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                )
                                        ); 
                                    */    
                                    -- Insert for Test Suit Project Relation - End

                                    -- Update Test Case -- Start
                                        UPDATE T_TEST_CASE_SUMMARY 
                                            SET T_TEST_CASE_SUMMARY.TEST_STEP_DESCRIPTION = (
                                                                                                SELECT DISTINCT tblstgtestcase.TESTCASEDESCRIPTION
                                                                                                FROM tblstgtestcase
                                                                                                WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                                                                    AND tblstgtestcase.DATASETMODE IS NULL 
                                                                                                     and tblstgtestcase.KEYWORD IS NOT NULL 
                                                                                                    AND NOT EXISTS (
                                                                                                        SELECT 1
                                                                                                        FROM TEMPFALSEAPPLICATION
                                                                                                        WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                                                            AND tempfalseapplication.FLAG = 0
                                                                                                            AND ROWNUM=1
                                                                                                    )
                                                                                                     AND ROWNUM=1
                                                                                            )
                                        WHERE t_test_case_summary.test_case_id IN   (
                                                                                        SELECT DISTINCT xy.test_case_id
                                                                                        FROM tblstgtestcase
                                                                                        INNER JOIN T_TEST_CASE_SUMMARY xy ON UPPER(xy.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                                                                        WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                                                            AND tblstgtestcase.DATASETMODE IS NULL 
                                                                                             and tblstgtestcase.KEYWORD IS NOT NULL 
                                                                                            AND NOT EXISTS (
                                                                                                SELECT 1
                                                                                                FROM TEMPFALSEAPPLICATION
                                                                                                WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                                                    AND tempfalseapplication.FLAG = 0
                                                                                                    AND ROWNUM=1
                                                                                            )
                                                                                             AND ROWNUM=1
                                                                                    );
                                    -- Update Test Case -- END

                                    -- Insert new Test Cases - Start
                                    --CHERISH
                                        --SELECT MAX(TEST_CASE_ID) INTO MAXRow FROM T_TEST_CASE_SUMMARY;    
                                        SELECT T_TEST_CASE_SUMMARY_SEQ.nextval INTO MAXRow FROM DUAL;    
                                        --remove distinct
                                        INSERT INTO T_TEST_CASE_SUMMARY (TEST_CASE_ID,TEST_CASE_NAME,TEST_STEP_DESCRIPTION,TEST_STEP_CREATOR,TEST_STEP_CREATE_TIME,USAGE_STATUS)
                                        SELECT  T_TEST_CASE_SUMMARY_SEQ.nextval AS ID,TESTCASENAME,TESTCASEDESCRIPTION,'SYSTEM',(SELECT SYSDATE FROM DUAL) AS CURRENTDATE,NULL
                                        FROM (
                                                SELECT DISTINCT tblstgtestcase.TESTCASENAME,tblstgtestcase.TESTCASEDESCRIPTION
                                                FROM tblstgtestcase
                                                WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                    AND tblstgtestcase.DATASETMODE IS NULL 
                                                     and tblstgtestcase.KEYWORD IS NOT NULL 
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM T_TEST_CASE_SUMMARY
                                                        WHERE T_TEST_CASE_SUMMARY.TEST_CASE_NAME = tblstgtestcase.TESTCASENAME
                                                        AND ROWNUM=1
                                                    )
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM TEMPFALSEAPPLICATION
                                                        WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                            AND tempfalseapplication.FLAG = 0
                                                            AND ROWNUM=1
                                                    )
                                        );        
                                    -- Insert new Test Cases - End

                                    -- Insert REL_TEST_CASE_TEST_SUITE -- Start
                                        INSERT INTO REL_TEST_CASE_TEST_SUITE (RELATIONSHIP_ID,TEST_CASE_ID,TEST_SUITE_ID)
                                        SELECT  (REL_TEST_CASE_TEST_SUITE_SEQ.NEXTVAL) AS ID, test_case_id, test_suite_id
                                        FROM ( 
                                            SELECT DISTINCT
                                                t_test_case_summary.test_case_id,
                                                t_test_suite.test_suite_id
                                            FROM tblstgtestcase
                                            INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN t_test_suite ON UPPER(t_test_suite.test_suite_name) = UPPER(tblstgtestcase.testsuitename)
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM REL_TEST_CASE_TEST_SUITE
                                                    WHERE REL_TEST_CASE_TEST_SUITE.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                        AND REL_TEST_CASE_TEST_SUITE.TEST_SUITE_ID = t_test_suite.test_suite_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                        ) xyz; 
                                    -- Insert REL_TEST_CASE_TEST_SUITE -- END

                                    -- Insert Test Data Set -- Start
                                    --CHERISH
                                        --SELECT MAX(DATA_SUMMARY_ID) INTO MAXRow FROM T_TEST_DATA_SUMMARY;
                                        SELECT T_TEST_STEPS_SEQ.nextval INTO MAXRow FROM DUAL;
                                        INSERT INTO T_TEST_DATA_SUMMARY(DATA_SUMMARY_ID,ALIAS_NAME,DESCRIPTION_INFO,AVAILABLE_MARK,VERSION,SHARE_MARK,CREATE_TIME,STATUS,DATA_SET_TYPE)
                                        SELECT  T_TEST_STEPS_SEQ.nextval AS ID,ALIAS_NAME,datasetdescription,AVAILABLE_MARK,VERSION,SHARE_MARK,CURRENTDATE,STATUS,DATA_SET_TYPE
                                        FROM (    
                                            SELECT DISTINCT 
                                                tblstgtestcase.DATASETNAME AS ALIAS_NAME,
                                                tblstgtestcase.datasetdescription,
                                                NULL AS AVAILABLE_MARK ,
                                                NULL AS VERSION,
                                                NULL AS SHARE_MARK,
                                                (SELECT SYSDATE FROM DUAL) AS CURRENTDATE,
                                                NULL AS STATUS,NULL as DATA_SET_TYPE
                                            FROM tblstgtestcase 
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM T_TEST_DATA_SUMMARY
                                                    WHERE t_test_data_summary.alias_name = tblstgtestcase.DATASETNAME
                                                    AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                            );    
                                    -- Insert Test Data Set -- End        

                                    -- Insert into REL_TC_DATA_SUMMARY - Start
                                    --CHERISH
                                        --SELECT MAX(ID) INTO MAXRow FROM REL_TC_DATA_SUMMARY;
                                        SELECT T_TEST_STEPS_SEQ.nextval INTO MAXRow FROM DUAL;

                                        INSERT INTO REL_TC_DATA_SUMMARY(ID,DATA_SUMMARY_ID,TEST_CASE_ID,CREATE_TIME)
                                        SELECT T_TEST_STEPS_SEQ.nextval AS ID,data_summary_id,test_case_id,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE
                                        FROM (    
                                            SELECT DISTINCT t_test_data_summary.data_summary_id,t_test_case_summary.test_case_id
                                            FROM tblstgtestcase
                                            INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                            INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM REL_TC_DATA_SUMMARY
                                                    WHERE REL_TC_DATA_SUMMARY.DATA_SUMMARY_ID = t_test_data_summary.data_summary_id
                                                        AND REL_TC_DATA_SUMMARY.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                        ); 
                                    -- Insert into REL_TC_DATA_SUMMARY - Start    

                                    -- Inser into Test Steps -- Start
                                    --CHERISH
                                        SELECT T_TEST_STEPS_SEQ.nextval INTO MAXRow FROM DUAL;
                                        --SELECT MAX(STEPS_ID)+1 INTO MAXRow FROM T_TEST_STEPS;

                                        INSERT INTO T_TEST_STEPS(STEPS_ID,RUN_ORDER,KEY_WORD_ID,TEST_CASE_ID,OBJECT_ID,COLUMN_ROW_SETTING,VALUE_SETTING,"COMMENT",IS_RUNNABLE,OBJECT_NAME_ID)
                                        SELECT T_TEST_STEPS_SEQ.nextval  AS ID,rownumber,key_word_id,test_case_id,object_id,parameter,NULL,comments,NULL,object_name_id
                                        FROM (    
                                            SELECT DISTINCT tblstgtestcase.rownumber,t_keyword.key_word_id,t_test_case_summary.test_case_id,t.object_id,tblstgtestcase.parameter,tblstgtestcase.comments,t.object_name_id
                                            FROM tblstgtestcase
                                            INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                            INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgtestcase.KEYWORD)
                                            CROSS JOIN TABLE(GETTOPOBJECT(tblstgtestcase.OBJECT, APPLICATIONID)) t
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM T_TEST_STEPS
                                                    WHERE T_TEST_STEPS.KEY_WORD_ID = t_keyword.key_word_id
                                                        AND T_TEST_STEPS.RUN_ORDER = tblstgtestcase.rownumber
                                                        AND T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                            ORDER BY t_test_case_summary.test_case_id,tblstgtestcase.rownumber    
                                        ); 
                                    -- Inser into Test Steps -- End

                                    -- Insert into T_SHARED_OBJECT_POOL -- Start
                                        SELECT MAX(OBJECT_POOL_ID) INTO MAXRow FROM  T_SHARED_OBJECT_POOL;

                                        INSERT INTO T_SHARED_OBJECT_POOL (OBJECT_POOL_ID,DATA_SUMMARY_ID,OBJECT_NAME,OBJECT_ORDER,LOOP_ID,DATA_VALUE,CREATE_TIME,VERSION)
                                        SELECT T_TEST_STEPS_SEQ.nextval AS ID,data_summary_id,object,rownumber,1,datasetvalue,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE, NULL
                                        FROM (    
                                            SELECT DISTINCT t_test_data_summary.data_summary_id,tblstgtestcase.object,tblstgtestcase.rownumber,tblstgtestcase.datasetvalue
                                            FROM tblstgtestcase
                                            INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                            INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgtestcase.KEYWORD)
                                            CROSS JOIN TABLE(GETTOPOBJECT(tblstgtestcase.OBJECT, APPLICATIONID)) t
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM T_SHARED_OBJECT_POOL
                                                    WHERE T_SHARED_OBJECT_POOL.DATA_SUMMARY_ID = t_test_data_summary.data_summary_id
                                                        AND NVL(T_SHARED_OBJECT_POOL.OBJECT_NAME, 'N/A') = NVL(tblstgtestcase.object, 'N/A')
                                                        AND T_SHARED_OBJECT_POOL.OBJECT_ORDER = tblstgtestcase.rownumber
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                                ORDER BY t_test_data_summary.data_summary_id, tblstgtestcase.rownumber
                                        ); 
                                    -- Insert into T_SHARED_OBJECT_POOL -- Start    

                                    -- Insert into TEST_DATA_SETTING - Start
                                    --CHERISH
                                       -- SELECT MAX(DATA_SETTING_ID) INTO MAXRow FROM TEST_DATA_SETTING;
                                        SELECT TEST_DATA_SETTING_SEQ.nextval INTO MAXRow FROM DUAL;

                                        INSERT INTO TEST_DATA_SETTING(DATA_SETTING_ID,STEPS_ID,LOOP_ID,DATA_VALUE,VALUE_OR_OBJECT,DESCRIPTION,DATA_SUMMARY_ID,DATA_DIRECTION,VERSION,CREATE_TIME,POOL_ID)
                                        SELECT  TEST_DATA_SETTING_SEQ.nextval AS ID,steps_id,1,datasetvalue,NULL,NULL,data_summary_id,skip,NULL,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE,object_pool_id
                                        FROM (    
                                            SELECT DISTINCT t_test_steps.steps_id,tblstgtestcase.datasetvalue,t_test_data_summary.data_summary_id,t_shared_object_pool.object_pool_id,tblstgtestcase.skip
                                            FROM tblstgtestcase
                                            INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                            INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgtestcase.KEYWORD)
                                            INNER JOIN T_SHARED_OBJECT_POOL ON T_SHARED_OBJECT_POOL.DATA_SUMMARY_ID = t_test_data_summary.data_summary_id
                                                AND NVL(T_SHARED_OBJECT_POOL.OBJECT_NAME,'N/A') = NVL(tblstgtestcase.object, 'N/A')
                                                AND T_SHARED_OBJECT_POOL.OBJECT_ORDER = tblstgtestcase.rownumber
                                            INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.KEY_WORD_ID = t_keyword.key_word_id
                                                AND T_TEST_STEPS.RUN_ORDER = tblstgtestcase.rownumber
                                                AND T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.test_case_id
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEST_DATA_SETTING
                                                    WHERE test_data_setting.steps_id = t_test_steps.steps_id
                                                        AND TEST_DATA_SETTING.DATA_SUMMARY_ID = t_test_data_summary.data_summary_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                                ORDER BY t_test_steps.steps_id
                                        ); 
                                    -- Insert into TEST_DATA_SETTING - End


                                    -- Insert into REL_APP_TESTCASE - Start
                                        SELECT MAX(RELATIONSHIP_ID) INTO MAXRow FROM REL_APP_TESTCASE;

                                        INSERT INTO REL_APP_TESTCASE(RELATIONSHIP_ID,APPLICATION_ID,TEST_CASE_ID)
                                        --VALUES(COUNT_REL_APP_TESTCASE,APPLICATIONID,GETTESTCASEID);
                                        SELECT REL_APP_TESTCASE_SEQ.nextval AS ID, application_id, test_case_id
                                        FROM (    
                                            SELECT DISTINCT t_registered_apps.application_id, t_test_case_summary.test_case_id
                                            FROM tblstgtestcase 
                                            INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                            INNER JOIN TEMPFALSEAPPLICATION ON UPPER(tempfalseapplication.testcasename) = UPPER(tblstgtestcase.testcasename)
                                                AND tempfalseapplication.FLAG = 1
                                            INNER JOIN T_REGISTERED_APPS ON t_registered_apps.app_short_name = tempfalseapplication.application
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM REL_APP_TESTCASE
                                                    WHERE REL_APP_TESTCASE.APPLICATION_ID = t_registered_apps.application_id
                                                        AND REL_APP_TESTCASE.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0 
                                                        AND ROWNUM=1
                                                )
                                        );     
                                    -- Insert into REL_APP_TESTCASE - End


                                    -- Insert into REL_APP_TESTSUITE - Start
                                        SELECT MAX(RELATIONSHIP_ID)+1 INTO MAXRow FROM REL_APP_TESTSUITE;

                                        INSERT INTO REL_APP_TESTSUITE(RELATIONSHIP_ID,APPLICATION_ID,TEST_SUITE_ID)
                                        --VALUES(COUNT_REL_APP_TESTCASE,APPLICATIONID,GETTESTCASEID);
                                        SELECT REL_APP_TESTSUITE_SEQ.nextval AS ID, application_id, test_suite_id
                                        FROM (    
                                            SELECT DISTINCT t_registered_apps.application_id, t_test_suite.test_suite_id
                                            --SELECT 1
                                            FROM tblstgtestcase 
                                            INNER JOIN t_test_suite ON UPPER(t_test_suite.test_suite_name) = UPPER(tblstgtestcase.testsuitename)
                                            INNER JOIN TEMPFALSEAPPLICATION ON UPPER(tempfalseapplication.testcasename) = UPPER(tblstgtestcase.testcasename)
                                                AND tempfalseapplication.FLAG = 1
                                            INNER JOIN T_REGISTERED_APPS ON t_registered_apps.app_short_name = tempfalseapplication.application
                                            WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgtestcase.DATASETMODE IS NULL 
                                                 and tblstgtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM REL_APP_TESTSUITE
                                                    WHERE REL_APP_TESTSUITE.APPLICATION_ID = t_registered_apps.application_id
                                                        AND rel_app_testsuite.test_suite_id = t_test_suite.test_suite_id
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0 
                                                        AND ROWNUM=1
                                                )
                                        );
                                    -- Insert into REL_APP_TESTSUITE - Start    
                                --dbms_output.put_line('All Insert Ended');
                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                --dbms_output.put_line(CURRENTTIME);
                            END;    

                            -- Performance Code -- Shivam - End

                        -- Code added by Shivam - To save the errorlog for tab with wrong application - Start
                            execute immediate 'TRUNCATE TABLE TEMP1';
                            execute immediate 'TRUNCATE TABLE TEMPSTGTESTCASE';
                            execute immediate 'TRUNCATE TABLE TEMPDWTESTCASE';

					--END; --- LOGIC FOR NOT OVERWRITE - END
---------------------------------------------------------------------------------------------------------- LOGIC FOR NOT OVERWRITE - END ----------------------------------------------------------------------------------------------------------
				--END IF; -- IF - 3 - END

				-------------- LOGIC FOR ANOTHER CURSOR WHERE DATASETMODE IS NOT NULL  --- START-----------------------------------

                -- Performance change by Shivam Parikh - END
-------------------------------------------------------------------------------
--------------------LOGIC FOR MODE D - START ----------------------------------

            -- Performance Code by Shivam Parikh - Start
            DECLARE 
                ERROR varchar2(5000);
                MESSAGE VARCHAR2(500);
                MaxRow INT;
            BEGIN

                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=17;

                -- Insert log for Test case and Test Date which are going to be deleted - Start
                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,xyz.LOGDETAILS,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                FROM (
                        SELECT DISTINCT T_TEST_CASE_SUMMARY.TEST_CASE_NAME ||','|| t_test_data_summary.alias_name || ',' || T_KEYWORD.KEY_WORD_NAME || ',' || T_REGISTED_OBJECT.OBJECT_HAPPY_NAME as LOGDETAILS,(SELECT SYSDATE FROM DUAL) as CREATEDON
                        FROM tblstgtestcase
                        INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name) = UPPER(TBLSTGTESTCASE.DATASETNAME)
                        INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(TEST_CASE_NAME)=UPPER(TBLSTGTESTCASE.TESTCASENAME)
                        INNER JOIN T_Test_Steps ON T_Test_Steps.TEST_CASE_ID = T_TEST_CASE_SUMMARY.TEST_CASE_ID
                        INNER JOIN T_KEYWORD ON T_KEYWORD.KEY_WORD_ID=T_Test_Steps.KEY_WORD_ID
                        INNER JOIN T_REGISTED_OBJECT ON T_REGISTED_OBJECT.OBJECT_ID=T_Test_Steps.OBJECT_ID
                        WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID 
                            AND DATASETMODE='D'

                ) xyz;
                -- Insert log for Test case and Test Date which are going to be deleted - Start

                -- Insert log for not found Test Case - Start
                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=14;

                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,MESSAGE,LOGDETAIL,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                FROM (    
                    SELECT DISTINCT tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.DATASETNAME AS LOGDETAIL
                    FROM tblstgtestcase
                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                        AND DATASETMODE='D'
                        AND NOT EXISTS (
                            SELECT 1
                            FROM T_TEST_CASE_SUMMARY
                            WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(tblstgtestcase.TESTCASENAME)
                        )
                ) xyz;
                -- Insert log for not found Test Case - End

                -- Insert log for not found Data Summary - Start

                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=15;

                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,MESSAGE,LOGDETAIL,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                FROM (    
                    SELECT DISTINCT tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.DATASETNAME AS LOGDETAIL
                    FROM tblstgtestcase
                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                        AND DATASETMODE='D'
                        AND NOT EXISTS (
                            SELECT 1
                            FROM T_TEST_DATA_SUMMARY
                            WHERE UPPER(t_test_data_summary.alias_name)=UPPER(tblstgtestcase.DATASETNAME)
                            AND ROWNUM=1
                        )
                ) xyz; 
                -- Insert log for not found Data Summary - End

                -- Insert log for not found Data Summary and Data Set both - Start

                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=16;

                INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
                SELECT I_INDEX.FILENAME,I_INDEX.FILETYPE,MESSAGE,LOGDETAIL,LOGREPORT_SEQ.NEXTVAL as ID,(SELECT SYSDATE FROM DUAL) as CREATEDON,I_INDEX.FEEDPROCESSDETAILID
                FROM (    
                    SELECT DISTINCT tblstgtestcase.TESTCASENAME  || '####' || tblstgtestcase.DATASETNAME AS LOGDETAIL
                    FROM tblstgtestcase
                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                        AND DATASETMODE='D'
                        AND NOT EXISTS (
                            SELECT 1
                            FROM T_TEST_DATA_SUMMARY
                            WHERE UPPER(t_test_data_summary.alias_name)=UPPER(tblstgtestcase.DATASETNAME)
                            AND ROWNUM=1
                        )
                        AND NOT EXISTS (
                            SELECT 1
                            FROM T_TEST_CASE_SUMMARY
                            WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(tblstgtestcase.TESTCASENAME) 
                            AND ROWNUM=1
                        )
                ) xyz; 
                -- Insert log for not found Data Summary and Data Set both - Start

                -- DELETE From tables - Start
                DELETE FROM TEST_DATA_SETTING
                WHERE TEST_DATA_SETTING.STEPS_ID IN 
                (
                    SELECT DISTINCT T_TEST_STEPS.STEPS_ID
                    FROM TBLSTGTESTCASE
                    INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name)=UPPER(TBLSTGTESTCASE.DATASETNAME)        
                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(TEST_CASE_NAME)=UPPER(TBLSTGTESTCASE.TESTCASENAME)
                    INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.TEST_CASE_ID=T_TEST_CASE_SUMMARY.TEST_CASE_ID
                    INNER JOIN test_data_setting ON test_data_setting.data_summary_id = t_test_data_summary.data_summary_id
                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                        AND DATASETMODE='D'
                );

                DELETE FROM REL_TC_DATA_SUMMARY
                WHERE REL_TC_DATA_SUMMARY.ID IN 
                (
                    SELECT DISTINCT REL_TC_DATA_SUMMARY.ID
                    FROM TBLSTGTESTCASE
                    INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name)=UPPER(TBLSTGTESTCASE.DATASETNAME)
                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(TEST_CASE_NAME)=UPPER(TBLSTGTESTCASE.TESTCASENAME)
                    INNER JOIN REL_TC_DATA_SUMMARY ON REL_TC_DATA_SUMMARY.TEST_CASE_ID = T_TEST_CASE_SUMMARY.TEST_CASE_ID
                        AND REL_TC_DATA_SUMMARY.DATA_SUMMARY_ID = T_TEST_DATA_SUMMARY.DATA_SUMMARY_ID
                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                        AND DATASETMODE='D'
                );
                -- DELETE From tables - End

            END;
            -- Performance Code by Shivam Parikh - End

-------------------------------------------------------------------------------
-------------------LOGIC FOR MODE D - END -------------------------------------







							-------------- LOGIC FOR ANOTHER CURSOR WHERE DATASETMODE IS NOT NULL  --- END-----------------------------------







							END;
							ELSE
								DECLARE
									AUTO_LOGREPORT NUMBER:=0;
								BEGIN
									SELECT MESSAGE INTO VALIDATEMESSAGE FROM TBLMESSAGE WHERE ID=11;
									SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

									INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
									VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,VALIDATEMESSAGE,'',AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
										IF RESULT IS NULL THEN
											RESULT:= 'ERROR';
										ELSE
											RESULT:= RESULT || '####' || 'ERROR';
										END IF;
								END;
						END IF; -- IF - 2 - END

									END;
								ELSE
									DECLARE
											AUTO_LOGREPORT NUMBER:=0;
									BEGIN

											-- ERROR MESSAGE OF APPLICATION ID IS NOT FOUND IN DATABASE
											SELECT MESSAGE INTO VALIDATEMESSAGE FROM TBLMESSAGE WHERE ID=12;
											SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

											INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON,FEEDPROCESSDETAILID)
											VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,VALIDATEMESSAGE,'',AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);
												IF RESULT IS NULL THEN
													RESULT:= 'ERROR';
												ELSE
													RESULT:= RESULT || '####' || 'ERROR';
												END IF;
									END;
								END IF;

							END;
						END IF;





------------------------------------------------------- END - CODE FOR IMPORT TEST CASE ---------------------------------
--------------------------------------------------------------------------------------------------------------------------


----------------------------------------------------- START - CODE FOR IMPORT Storyboard ---------------------------------
-------------------------------------------------------------------------------------------------------------------------
 IF I_INDEX.FILETYPE = 'STORYBOARD' THEN
          DECLARE
            CURRENTDATE TIMESTAMP;
        BEGIN
		SELECT SYSDATE INTO CURRENTDATE FROM DUAL;  
         insert into Storyboard_Insert_Temp (Name,Type)        
        select distinct regexp_substr(applicationname,'[^,]+', 1, level), 'APPLICATION' from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
        connect by regexp_substr(applicationname, '[^,]+', 1, level) is not null; 

        insert into Storyboard_Insert_Temp(Name,Type,Description)
        select distinct projectname,'PROJECT',projectdetail from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

        insert into Storyboard_Insert_Temp(Name,Type,Description)
        select distinct storyboardname,'STORYBOARD',projectname from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

        insert into Storyboard_Insert_Temp (Name,Type,Description)        
        select distinct regexp_substr(applicationname,'[^,]+', 1, level), 'Mapping',projectname from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
        connect by regexp_substr(applicationname, '[^,]+', 1, level) is not null; 

        insert into Storyboard_Insert_Temp(Name,Type,Description)
        select distinct suitename,'MappingTestSuiteProject',projectname from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;




         update t_test_project  tt
         set  tt.project_description = (select  temp.description from Storyboard_Insert_Temp temp where temp.name = tt.project_name and temp.Type = 'PROJECT' and ROWNUM = 1
)
         where Exists (select temp.name from Storyboard_Insert_Temp temp where temp.name = tt.project_name and temp.Type = 'PROJECT' and ROWNUM = 1);





            insert into t_test_project(Project_Id,project_name,project_description,creator,create_date,status)
             select T_TEST_PROJECT_SEQ.nextval,temp.name,temp.description,'System',CURRENTDATE,2 from Storyboard_Insert_Temp temp
            left join t_test_project tt on temp.name = tt.project_name
            where tt.project_id is null and temp.Type = 'PROJECT';




             insert into t_storyboard_summary(storyboard_id,assigned_project_id,storyboard_name)
             select T_TEST_STEPS_SEQ.nextval,  prj.project_id,temp.name from Storyboard_Insert_Temp temp
              join t_test_project prj on prj.project_name = temp.description 
            left join t_storyboard_summary tt on temp.name = tt.storyboard_name and prj.project_id = tt.assigned_project_id

            where tt.storyboard_id is null and temp.Type = 'STORYBOARD';



            insert into REL_APP_PROJ(relationship_id,application_id,project_id)         
         select REL_APP_PROJ_SEQ.nextval,apps.application_id,prj.project_id from Storyboard_Insert_Temp stg
         join t_test_project prj on prj.project_name = stg.description
         join t_registered_apps apps on apps.app_short_name = stg.name
         left join REL_APP_PROJ rels on rels.application_id = apps.application_id and rels.project_id = prj.project_id
         where rels.relationship_id is null and
         stg.type = 'Mapping';


          insert into REL_TEST_SUIT_PROJECT(relationship_id,test_suite_id,project_id)         
         select T_TEST_STEPS_SEQ.nextval,suite.test_suite_id,prj.project_id from Storyboard_Insert_Temp stg
         join t_test_project prj on prj.project_name = stg.description
         join t_test_suite suite on suite.test_suite_name = stg.name
         left join REL_TEST_SUIT_PROJECT rels on rels.test_suite_id = suite.test_suite_id and rels.project_id = prj.project_id
         where rels.relationship_id is null and
         stg.type = 'MappingTestSuiteProject';





         delete T_STORYBOARD_DATASET_SETTING where storyboard_detail_id in
         (select mgr.storyboard_detail_id from tblstgstoryboard sa 
            join t_test_project prj  on sa.projectname = prj.project_name
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
          join T_PROJ_TC_MGR mgr on mgr.storyboard_id = tc.storyboard_id and mgr.project_id= prj.project_id
          where sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID 
         ) ;

         --delete from T_PROJ_TC_MGR
         delete T_PROJ_TC_MGR where storyboard_detail_id in
         (select mgr.storyboard_detail_id from tblstgstoryboard sa 
            join t_test_project prj  on sa.projectname = prj.project_name
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
          join T_PROJ_TC_MGR mgr on mgr.storyboard_id = tc.storyboard_id and mgr.project_id= prj.project_id
          where sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID 
         ) ;


           insert into  T_PROJ_TC_MGR(storyboard_detail_id,project_id,test_case_id,storyboard_id,run_type,run_order,test_suite_id,alias_name)
            select T_TEST_STEPS_SEQ.nextval ,prj.project_id,tcs.test_case_id,tc.storyboard_id,lkp.value,sa.runorder,trs.test_suite_id,sa.stepname from tblstgstoryboard sa                        
            join t_test_suite trs on trs.test_suite_name = sa.suitename
            join t_test_case_summary tcs on tcs.test_case_name = sa.casename
            join t_test_project prj  on sa.projectname = prj.project_name      

            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on lkp.display_name = sa.actionname and lkp.field_name = 'RUN_TYPE'
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID ;

            update T_PROJ_TC_MGR mgr
            set mgr.depends_on = (select mgr2.storyboard_detail_id from tblstgstoryboard sa                        
            join t_test_suite trs on trs.test_suite_name = sa.suitename
            join t_test_case_summary tcs on tcs.test_case_name = sa.casename
            join t_test_project prj  on sa.projectname = prj.project_name      
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on lkp.display_name = sa.actionname and lkp.field_name = 'RUN_TYPE'            
            join T_PROJ_TC_MGR mgr2 on tc.storyboard_id = mgr2.storyboard_id and prj.project_id = mgr2.project_id and mgr2.alias_name = sa.dependency
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
            and tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id and trs.test_suite_id = mgr.test_suite_id 
                        and tcs.test_case_id = mgr.test_case_id  and mgr.run_type = lkp.value and sa.runorder = mgr.run_order
            )
            where exists (select mgr2.storyboard_detail_id from tblstgstoryboard sa                        
            join t_test_suite trs on trs.test_suite_name = sa.suitename
            join t_test_case_summary tcs on tcs.test_case_name = sa.casename
            join t_test_project prj  on sa.projectname = prj.project_name      
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on lkp.display_name = sa.actionname and lkp.field_name = 'RUN_TYPE'            
            join T_PROJ_TC_MGR mgr2 on tc.storyboard_id = mgr2.storyboard_id and prj.project_id = mgr2.project_id and mgr2.alias_name = sa.dependency
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
            and tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id and trs.test_suite_id = mgr.test_suite_id 
                        and tcs.test_case_id = mgr.test_case_id  and mgr.run_type = lkp.value and sa.runorder = mgr.run_order);            





           insert into  T_STORYBOARD_DATASET_SETTING(setting_id,  storyboard_detail_id,data_summary_id)
            select T_TEST_STEPS_SEQ.nextval ,mgr.storyboard_detail_id,tr.data_summary_id from tblstgstoryboard sa                        
            join t_test_data_summary tr on tr.alias_name = sa.datasetname
            join t_test_suite trs on trs.test_suite_name = sa.suitename
            join t_test_case_summary tcs on tcs.test_case_name = sa.casename
            join t_test_project prj  on sa.projectname = prj.project_name
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on lkp.display_name = sa.actionname and lkp.field_name = 'RUN_TYPE'
            join T_PROJ_TC_MGR mgr on tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id and trs.test_suite_id = mgr.test_suite_id 
                        and tcs.test_case_id = mgr.test_case_id
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID ;


        End;             
        End If;
        commit;

------------------------------------------------------- END - CODE FOR IMPORT Storyboard ---------------------------------
--------------------------------------------------------------------------------------------------------------------------


----------------------------------------------------- START - CODE FOR IMPORT Variable ---------------------------------
-------------------------------------------------------------------------------------------------------------------------

IF I_INDEX.FILETYPE = 'VARIABLE' THEN
          DECLARE
            CURRENTDATE TIMESTAMP;
        --    ApplicationName VARCHAR2(5000);
        -- Remove duplicate staging rows
        BEGIN
        DELETE FROM tblstgvariable
           WHERE ID IN (
             SELECT DISTINCT ID
             from tblstgvariable t
             where t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
             AND EXISTS (
               SELECT 1
               FROM tblstgvariable s
               WHERE s.ID > t.ID
               AND s.feedprocessdetailid = t.feedprocessdetailid
               AND (               
              s.feedprocessdetailid = s.feedprocessdetailid 
              and NVL(s.name, 'N/A') = NVL(t.name, 'N/A')
              and NVL(s.type, 'N/A') = NVL(t.type, 'N/A')              
              and NVL(s.base_comp, 'N/A') = NVL(t.base_comp, 'N/A')
               )
            )
        );    
        commit;    


         --region Project
        -- update Global Variable  and Loop/Modal Variable     
         update system_lookup  lkp
         set  lkp.display_name = (select upper(temp.value) from tblstgvariable temp where upper(temp.name) = upper(lkp.field_name) and upper(temp.type) = 'GLOBAL_VAR' and temp.Feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID )
         where Exists (select temp.value from tblstgvariable temp where upper(temp.name) = upper(lkp.field_name) and upper(temp.type) = 'GLOBAL_VAR' and temp.Feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID)
         and lkp.table_name = 'GLOBAL_VAR';

         update system_lookup  lkp
         set  lkp.display_name = (select upper(temp.value) from tblstgvariable temp where upper(temp.name) = upper(lkp.field_name) and upper(temp.type) in ('MODAL_VAR','LOOP_VAR') 
                and temp.base_comp_id = lkp.status and temp.Feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID)
         where Exists (select temp.value from tblstgvariable temp where upper(temp.name) = upper(lkp.field_name) and upper(temp.type) in ('MODAL_VAR','LOOP_VAR')
                and temp.base_comp_id = lkp.status and temp.Feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID)
         and lkp.table_name in ('MODAL_VAR','LOOP_VAR') ;

         -- insert into Variable 

            insert into system_lookup(lookup_id,table_name,field_name,value,display_name,status)           
            select SYSTEM_LOOKUP_SEQ.nextval ,upper(temp.type) ,upper(temp.name),1,upper(temp.value) ,temp.base_comp_id from tblstgvariable temp
            left join system_lookup lkp on upper(temp.name) = upper(lkp.field_name) and lkp.table_name in ('MODAL_VAR','LOOP_VAR','GLOBAL_VAR')
            where lkp.lookup_id is null  and upper(temp.Type) in ('MODAL_VAR','LOOP_VAR','GLOBAL_VAR')
            and temp.Feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

         -- endregion for Variable


         --endregion
        End;             
        End If;------------------------------------------------------- END - CODE FOR IMPORT Variable ---------------------------------
--------------------------------------------------------------------------------------------------------------------------

      -- Remove all duplicate Data set mappings in the import - Start
        DELETE FROM REL_TC_DATA_SUMMARY
        WHERE ID IN (
          SELECT DISTINCT
            rdup.ID
          FROM REL_TC_DATA_SUMMARY r
          INNER JOIN REL_TC_DATA_SUMMARY rdup ON r.TEST_CASE_ID = r.TEST_CASE_ID
            AND r.DATA_SUMMARY_ID = r.DATA_SUMMARY_ID
            AND rdup.ID > r.ID
          INNER JOIN T_TEST_DATA_SUMMARY ts ON ts.DATA_SUMMARY_ID = r.DATA_SUMMARY_ID
          INNER JOIN T_TEST_DATA_SUMMARY tsdup ON tsdup.DATA_SUMMARY_ID = rdup.DATA_SUMMARY_ID
            AND ts.ALIAS_NAME = tsdup.ALIAS_NAME
            AND NVL(ts.DESCRIPTION_INFO, 'N/A') = NVL(ts.DESCRIPTION_INFO, 'N/A')
          INNER JOIN T_TEST_CASE_SUMMARY tc ON tc.TEST_CASE_ID = r.TEST_CASE_ID
          INNER JOIN T_TEST_CASE_SUMMARY tcdup ON tcdup.TEST_CASE_ID = rdup.TEST_CASE_ID
            AND tc.TEST_CASE_NAME = tc.TEST_CASE_NAME
          INNER JOIN REL_TEST_CASE_TEST_SUITE rtcts ON rtcts.TEST_CASE_ID = tc.TEST_CASE_ID
            AND rtcts.TEST_CASE_ID = tcdup.TEST_CASE_ID  
          INNER JOIN T_TEST_SUITE tss ON tss.TEST_SUITE_ID = rtcts.TEST_SUITE_ID
          INNER JOIN tblstgtestcase stg ON stg.TESTSUITENAME = tss.TEST_SUITE_NAME
          INNER JOIN TBLFEEDPROCESSDETAILS fpd ON fpd.FEEDPROCESSDETAILID = stg.FEEDPROCESSDETAILID
            AND fpd.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
          WHERE   rdup.ID > r.ID
        );
      -- Remove all duplicate Data set mappings in the import - End


			END LOOP;
	--CLOSE FORTBLFEEDPROCESS;
END;
--execute immediate 'TRUNCATE TABLE tblstgtestcase';
BEGIN
    EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_LAST_TC_INFO COMPILE';
    EXCEPTION
    WHEN OTHERS THEN 
    EXECUTE IMMEDIATE 'ALTER VIEW MV_LAST_TC_INFO COMPILE';
END;   
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_LATEST_STB_TSMOD_MARK  COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_MARS_STB_SNAPSHOT_SUB COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_STORYBOARD_LATEST COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_TC_DATASUMMARY COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW V_OBJECT_SNAPSHOT COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_OBJ_WITH_PEG COMPILE';

END USP_FEEDPROCESSMAPPING_Mode_D;
/

create or replace PROCEDURE          USP_MAPPING_VALIDATION(
    FEEDPROCESSID1 IN NUMBER,
   --ISOVERWRITE IN NUMBER,
    RESULT OUT CLOB
    
)
IS 
BEGIN
		DECLARE

            CHECKRESULT  NUMBER:=0;
			CURSOR FORTBLFEEDPROCESS IS 
		    SELECT  FEEDPROCESSDETAILID,FEEDPROCESSID,FILENAME,FEEDPROCESSSTATUS,CREATEDBY,CREATEDON,FILETYPE FROM TBLFEEDPROCESSDETAILS WHERE FEEDPROCESSID = FEEDPROCESSID1;

		BEGIN

			--OPEN FORTBLFEEDPROCESS;

			FOR I_INDEX IN FORTBLFEEDPROCESS
			LOOP


						---------------------------- START - CODE FOR IMPORT OBJECT FILE ---------------------------------
						--------------------------------------------------------------------------------------------------
						IF I_INDEX.FILETYPE = 'OBJECT' THEN
  ------- LOGIC FOR VALIDATE ORDER - START ---------------------------------------------------------------------------------------------------------
  DECLARE
     VALIDATEMESSAGE VARCHAR2(5000);
     VALIDATETYPE VARCHAR2(5000);
     APPLICATIONID INT:=0;
  BEGIN
      SELECT MESSAGE INTO VALIDATEMESSAGE FROM TBLMESSAGE WHERE ID=23; 

      SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=23; 
      INSERT INTO "TBLLOGREPORT"(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,APPLICATIONNAME,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
      SELECT (SELECT SYSDATE FROM DUAL),VALIDATETYPE,'Validation','','ORDER OF OBJECT IS INVALID',VALIDATEMESSAGE,c.APPLICATIONNAME,'','','',c.OBJECTNAME,c.PARENT,VALIDATEMESSAGE,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1
      FROM tblStgGuiObject c
      WHERE c.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
        AND c.PARENT <> c.OBJECTNAME
        AND NOT EXISTS (
          SELECT 1
          FROM tblStgGuiObject p
          WHERE p.OBJECTNAME = c.PARENT
            AND c.TYPE <> 'Pegwindow'
            AND p.TYPE = 'Pegwindow'
            AND c.ID > p.ID
            AND c.OBJECTNAME <> p.OBJECTNAME
            AND p.OBJECTNAME = p.PARENT
            AND p.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
        );
         END;
  --------LOGIC FOR VALIDATE ORDER - END -----------------------------------------------------------------------------------------------------------

							DECLARE

							GETTYPEID NUMBER:=0; -- USE IN IMPORT OBJECT
							NEWAPPLICATION_ID NUMBER:=0;
							FULLDATA CLOB;
							GETOBJECTHAPPYNAME VARCHAR2(500):=NULL;
							GETOBJECTNAMEID NUMBER;
							GETOBJECTPARENT VARCHAR2(500):=NULL;
							GETOBJECTPARENTID NUMBER;
                            GETCOUNTOFAPPS NUMBER:=0;
							CURRENTDATE TIMESTAMP;

							CURSOR FORTBLSTGGUIOBJECT IS
							SELECT OBJECTNAME,TYPE,QUICKACCESS,PARENT,OBJECTCOMMENT,ENUMTYPE,SQL,APPLICATIONNAME,FEEDPROCESSDETAILID FROM TBLSTGGUIOBJECT WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

							BEGIN
									FOR OBJ_INDEX IN FORTBLSTGGUIOBJECT
									LOOP

									   --SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

                    -- FULLDATA := (OBJ_INDEX.OBJECTNAME || '####' || OBJ_INDEX.TYPE || '####' || OBJ_INDEX.QUICKACCESS || '####' || OBJ_INDEX.PARENT || '####' || OBJ_INDEX.OBJECTCOMMENT || '####' || OBJ_INDEX.ENUMTYPE || '####' || OBJ_INDEX."SQL" || '####' || OBJ_INDEX.APPLICATIONNAME || '####' || OBJ_INDEX.FEEDPROCESSDETAILID); 

										 -- CHECK APPLICATION ID IS EXSITS OR NOT
                      SELECT COUNT(*) INTO GETCOUNTOFAPPS FROM T_REGISTERED_APPS WHERE UPPER(APP_SHORT_NAME)=UPPER(OBJ_INDEX.APPLICATIONNAME) ; -- TO GET THE NEXT VALUE


										 IF GETCOUNTOFAPPS != 0 THEN
                        DECLARE
                        GETTYPEIDCOUNT NUMBER:=0;
												BEGIN
													--- CHECK TYPE_ID IS IN TABLE T_GUI_COMPONENT_TYPE_DIC IF NOT FOUND THEN STOP
                          SELECT APPLICATION_ID INTO NEWAPPLICATION_ID FROM T_REGISTERED_APPS WHERE UPPER(APP_SHORT_NAME)=UPPER(OBJ_INDEX.APPLICATIONNAME) ; -- TO GET THE NEXT VALUE
													SELECT COUNT(*)  INTO GETTYPEIDCOUNT FROM T_GUI_COMPONENT_TYPE_DIC WHERE UPPER(TYPE_NAME) = UPPER(OBJ_INDEX.TYPE);
                          --dbms_output.put_line('TYPE_ID :->'||GETTYPEID);
    											IF  GETTYPEIDCOUNT != 0 THEN
                            declare
                            checkisparent number:=0;
                            checkobjectname number:=0;
														BEGIN
                            SELECT TYPE_ID  INTO GETTYPEID FROM T_GUI_COMPONENT_TYPE_DIC WHERE UPPER(TYPE_NAME) = UPPER(OBJ_INDEX.TYPE);

														--- CHECK PARENT IN TABLE : T_OBJECT_NAMEINFO IF NOT THEN CREATE AND GET THAT ID AND NAME VALUE PAIR
                            if  OBJ_INDEX.PARENT is null then
                            begin
                              select count(*) into checkisparent from T_OBJECT_NAMEINFO where  UPPER(T_OBJECT_NAMEINFO.object_happy_name)=UPPER(OBJ_INDEX.parent) and rownum=1;
                              if checkisparent =0 then
															BEGIN
                                SELECT MAX(OBJECT_NAME_ID)+1  INTO GETOBJECTPARENTID FROM T_OBJECT_NAMEINFO;

																--INSERT INTO "T_OBJECT_NAMEINFO" (OBJECT_NAME_ID,OBJECT_HAPPY_NAME,PEGWINDOW_MARK,OBJNAME_DESCRIPTION)
																--VALUES(GETOBJECTPARENTID,OBJ_INDEX.OBJECTNAME,0,NULL);
            --
																--SELECT OBJECT_HAPPY_NAME  INTO GETOBJECTPARENT FROM T_OBJECT_NAMEINFO WHERE T_OBJECT_NAMEINFO.OBJECT_NAME_ID=GETOBJECTPARENTID;
                              END;
                              else
                              begin
                                SELECT OBJECT_HAPPY_NAME INTO GETOBJECTPARENT FROM T_OBJECT_NAMEINFO WHERE   UPPER(T_OBJECT_NAMEINFO.object_happy_name)=UPPER(OBJ_INDEX.PARENT) and rownum=1;
															END;
                              end if;
                            end;
														END IF;


																--- CHECK OBJECT_HAPPY_NAME IN TABLE : T_OBJECT_NAMEINFO IF NOT THEN CREATE AND GET THAT ID AND NAME VALUE PAIR

                            SELECT count(*) INTO checkobjectname FROM T_OBJECT_NAMEINFO WHERE UPPER(OBJECT_HAPPY_NAME)=UPPER(OBJ_INDEX.OBJECTNAME) and rownum=1;

                            if checkobjectname =0 then
														BEGIN
                              SELECT MAX(OBJECT_NAME_ID)+1  INTO GETOBJECTNAMEID FROM T_OBJECT_NAMEINFO;

																			--INSERT INTO "T_OBJECT_NAMEINFO" (OBJECT_NAME_ID,OBJECT_HAPPY_NAME,PEGWINDOW_MARK,OBJNAME_DESCRIPTION)
																			--VALUES(GETOBJECTNAMEID,OBJ_INDEX.OBJECTNAME,0,NULL);

														END;
														ELSE
														BEGIN
                              SELECT OBJECT_NAME_ID INTO GETOBJECTNAMEID FROM T_OBJECT_NAMEINFO WHERE UPPER(OBJECT_HAPPY_NAME)=UPPER(OBJ_INDEX.OBJECTNAME) and rownum=1;
														END;

														END IF;
                          END;
													ELSE
														DECLARE
															ERROR VARCHAR2(500);
															ERROR_TYPE VARCHAR2(500);
													BEGIN
                            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=29;
                            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=29;

                            INSERT INTO "TBLLOGREPORT"(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,APPLICATIONNAME,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
                            VALUES((SELECT SYSDATE FROM DUAL),ERROR_TYPE,'Validation','','OBJECT TYPE ID IS INVALID',ERROR,'','','','',OBJ_INDEX.OBJECTNAME,OBJ_INDEX.PARENT,ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1);

															-- SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

														--	INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON, feedprocessdetailid)
														--	VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,FULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);

													END;

													END IF;
                        END;
                        ELSE
												DECLARE
														ERROR VARCHAR2(500);
														ERROR_TYPE VARCHAR2(500);
												BEGIN
														SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=2;
                            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=2;

														--SELECT  LOGREPORT_SEQ.NEXTVAL INTO AUTO_LOGREPORT FROM DUAL;

                            INSERT INTO "TBLLOGREPORT"(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,APPLICATIONNAME,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)

                            VALUES((SELECT SYSDATE FROM DUAL),ERROR_TYPE,'Validation','','Application Name Validation',ERROR,'','','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1);
														--INSERT INTO "LOGREPORT"(FILENAME,OBJECT,STATUS,LOGDETAILS,ID,CREATEDON, feedprocessdetailid)
														--VALUES(I_INDEX.FILENAME,I_INDEX.FILETYPE,ERROR,FULLDATA,AUTO_LOGREPORT,CURRENTDATE,I_INDEX.FEEDPROCESSDETAILID);

												END;
									   END IF;

								END LOOP;
								--CLOSE FORTBLSTGGUIOBJECT;
							END;	

						END IF;			
------------------------------------------------------ END - CODE FOR IMPORT OBJECT FILE ---------------------------------
---------------------------------------------------------------------------------------------------------------			

----------------------------------------------------- START - CODE FOR IMPORT COMPAREPARAM ---------------------------------
----------------------------------------------------------------------------------------------------------------------------
						IF I_INDEX.FILETYPE = 'COMPAREPARAM' THEN

							BEGIN
								DECLARE

								COMPAREPARAMFULLDATA CLOB;
								GETDATASOURCENAME VARCHAR2(500):=NULL;
								GETDATASOURCETYPEID NUMBER:=0;
								ERROR VARCHAR2(500);
								CURRENTDATE TIMESTAMP;

								CURSOR FORTBLSTGCOMPAREPARAM IS
								SELECT DATASOURCENAME,DATASOURCETYPE,DETAILS FROM TBLSTGCOMPAREPARAM WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

								BEGIN

									FOR COMPARE_INDEX IN FORTBLSTGCOMPAREPARAM
									LOOP


										-- 1 -- COMPARE -- 2 -- CONNECTION  -- 3 -- QUERY -- 4 -- PROFILE

										IF COMPARE_INDEX.DATASOURCETYPE NOT IN (1,2,3,4) THEN
											DECLARE



                                                VALIDATEMESSAGE VARCHAR2(5000);
                                                VALIDATETYPE VARCHAR2(5000);
											BEGIN
                                                SELECT MESSAGE INTO VALIDATEMESSAGE FROM TBLMESSAGE WHERE ID=3;
                                                SELECT messagesymbol INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=3;

                                                INSERT INTO "TBLLOGREPORT"(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,APPLICATIONNAME,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
                                                VALUES((SELECT SYSDATE FROM DUAL),VALIDATETYPE,'Validation','','DATA SOURCE TYPE IS INVALID',VALIDATEMESSAGE,'','','','','','',VALIDATEMESSAGE,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1);

											END;
										END IF;
									END LOOP;
									--CLOSE FORTBLSTGCOMPAREPARAM;
								END;

							END;

						END IF;

----------------------------------------------------- END - CODE FOR IMPORT COMPAREPARAM ---------------------------------
--------------------------------------------------------------------------------------------------------------------------

----------------------------------------------------- START - CODE FOR IMPORT TEST CASE ---------------------------------
-------------------------------------------------------------------------------------------------------------------------


						IF I_INDEX.FILETYPE = 'TESTCASE' THEN
							DECLARE
									VALIDATEMESSAGE VARCHAR2(5000);
									TESTSUITECOUNT NUMBER:=0;
									APPLICATIONID NUMBER:=0;
									COUNT_APPLICATIONID NUMBER:=0;
									CURRENTDATE TIMESTAMP;
									TESTSUITENAME VARCHAR(5000);
					                TESTCASENAME VARCHAR(5000);
                                    VALIDATETYPE VARCHAR2(5000);
							BEGIN

                                    -- Remove duplicate staging rows

                                    DELETE FROM tblstgtestcase
                                    WHERE ID IN (
                                        SELECT DISTINCT ID
                                        from tblstgtestcase t
                                        where t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                        AND EXISTS (
                                            SELECT 1
                                            FROM tblstgtestcase s
                                            WHERE s.ID > t.ID
                                                AND s.feedprocessdetailid = t.feedprocessdetailid
                                                AND (
                                                    --t.TESTSUITENAME = s.TESTSUITENAME AND  t.TESTCASENAME = s.TESTCASENAME  AND t.TESTCASEDESCRIPTION = s.TESTCASEDESCRIPTION AND NVL(t.DATASETMODE, 'N/A') = NVL(s.DATASETMODE, 'N/A') AND  t.KEYWORD = s.KEYWORD AND  NVL(t.OBJECT, 'N/A') = NVL(s.OBJECT, 'N/A') AND  NVL(t.PARAMETER, 'N/A') = NVL(s.PARAMETER, 'N/A') AND NVL(t.COMMENTS, 'N/A') = NVL(s.COMMENTS, 'N/A') AND t.DATASETNAME = s.DATASETNAME AND  t.DATASETVALUE = s.DATASETVALUE AND  t.ROWNUMBER = s.ROWNUMBER AND  t.FEEDPROCESSDETAILID = s.FEEDPROCESSDETAILID AND  t.TABNAME = s.TABNAME AND  t.APPLICATION = s.APPLICATION AND  t.CREATEDON = s.CREATEDON
                                                    t.TESTSUITENAME = s.TESTSUITENAME AND  t.TESTCASENAME = s.TESTCASENAME  AND t.TESTCASEDESCRIPTION = s.TESTCASEDESCRIPTION AND NVL(t.DATASETMODE, 'N/A') = NVL(s.DATASETMODE, 'N/A') AND  t.KEYWORD = s.KEYWORD AND  NVL(t.OBJECT, 'N/A') = NVL(s.OBJECT, 'N/A') AND  NVL(t.PARAMETER, 'N/A') = NVL(s.PARAMETER, 'N/A') AND NVL(t.COMMENTS, 'N/A') = NVL(s.COMMENTS, 'N/A') AND t.DATASETNAME = s.DATASETNAME AND  NVL(t.DATASETVALUE, 'N/A') = NVL(s.DATASETVALUE, 'N/A') AND  t.FEEDPROCESSDETAILID = s.FEEDPROCESSDETAILID AND  t.TABNAME = s.TABNAME AND  t.APPLICATION = s.APPLICATION AND s.rownumber = t.rownumber
                                                )
                                        )
                                    );    
                                    commit;    

									SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

									SELECT COUNT(*) INTO COUNT_APPLICATIONID FROM T_REGISTERED_APPS,TBLSTGTESTCASE WHERE  
									T_REGISTERED_APPS.APP_SHORT_NAME= TBLSTGTESTCASE.APPLICATION
									AND TBLSTGTESTCASE.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

								IF (COUNT_APPLICATIONID = 0) THEN
									BEGIN
                                        SELECT count(*) INTO COUNT_APPLICATIONID
                                        FROM (
                                            SELECT Application 
                                            FROM (
                                                    SELECT 
                                                        trim(regexp_substr(Application, '[^,]+', 1, level)) Application
                                                    FROM (
                                                        SELECT DISTINCT Application 
                                                        FROM TBLSTGTESTCASE 
                                                        WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                    ) t
                                                    CONNECT BY instr(Application, ',', 1, level - 1) > 0
                                            ) xy

                                            WHERE EXISTS (
                                                SELECT 1
                                                FROM T_REGISTERED_APPS 
                                                WHERE T_REGISTERED_APPS.APP_SHORT_NAME = xy.Application
                                            )
                                        ) xyz;

									end;
                                END if;		    

								IF 1 = 1 THEN -- IF COUNT_APPLICATIONID!=0 THEN
									DECLARE
										AUTO_TEMP1 NUMBER:=0;
                    APPLICATIONC NUMBER:=0;
									BEGIN

                                        dbms_output.put_line('COUNT_APPLICATIONID '||COUNT_APPLICATIONID);
                                        -- Code developed by Shivam -- Start
                                        SELECT DISTINCT upper(TBLSTGTESTCASE.TESTSUITENAME) INTO TESTSUITENAME
                                                                FROM TBLSTGTESTCASE
                                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID
                                                                    AND ROWNUM = 1;
                                         -- Code for inserting folse Application in the temp table.
                                         -- Delete data from temporary table
                                         DELETE FROM TEMPFALSEAPPLICATION;
                                        -- Insert the false data
                                        INSERT INTO TEMPFALSEAPPLICATION
                                        (
                                            TESTSUITNAME,
                                            TESTCASENAME,
                                            APPLICATION,
                                            Flag
                                        )
                                        SELECT TESTSUITENAME,x.TESTCASENAME,Application,CASE WHEN EXISTS 
                                        (SELECT 1 FROM T_REGISTERED_APPS WHERE T_REGISTERED_APPS.APP_SHORT_NAME = x.Application) THEN 1 ELSE 0 END
                                        FROM (
                                            SELECT DISTINCT
                                                trim(regexp_substr(Application, '[^,]+', 1, level)) Application,testcasename
                                            FROM (
                                                    SELECT DISTINCT Application, tblstgtestcase.testcasename
                                                    FROM TBLSTGTESTCASE 
                                                    WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                 ) t
                                            CONNECT BY instr(Application, ',', 1, level - 1) > 0
                                            order by Application
                                        ) x;
                                        -- GET TOP 1 Application_ID
                                        SELECT COUNT(1) INTO APPLICATIONC
                                        FROM T_REGISTERED_APPS
                                        WHERE APP_SHORT_NAME IN (
                                          SELECT APPLICATION
                                          FROM TEMPFALSEAPPLICATION
                                          WHERE ROWNUM = 1
                                        );
                                        IF (APPLICATIONC > 0)
                                        THEN
                                        BEGIN
                                          SELECT NVL(APPLICATION_ID, 0) INTO APPLICATIONID
                                          FROM T_REGISTERED_APPS
                                          WHERE APP_SHORT_NAME IN (
                                            SELECT APPLICATION
                                            FROM TEMPFALSEAPPLICATION
                                            WHERE ROWNUM = 1
                                          );
                                        END;
                                        END IF;
                                        -- 
                                        -- Code developed by Shivam -- End
                                        --delete from TEMP1;
                                        SELECT  TEMP1_SEQ.NEXTVAL INTO AUTO_TEMP1 FROM DUAL;

                                        --INSERT INTO TEMP1 (TESTCASENAME)
                                        --SELECT TESTSUITENAME FROM TBLSTGTESTCASE  WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID GROUP BY  TESTSUITENAME;

                                        --SELECT COUNT(*) INTO TESTSUITECOUNT FROM TEMP1;
                                        --dbms_output.put_line('TESTSUITECOUNT '||TESTSUITECOUNT);

                                        IF 1 = 1 THEN -- IF - 2 -START -- IF TESTSUITECOUNT = 1 THEN -- IF - 2 -START
                                            DECLARE
                                                        AUTO_TEMP1 NUMBER:=0;
                                                        AUTO_TEMPDWTESTCASE NUMBER:=0;  -- UNDER OBSERVATION
                                                        AUTO_TEMPSTGTESTCASE NUMBER:=0; -- UNDER OBSERVATION
                                                        CURRENTDATE TIMESTAMP;

                                                BEGIN

                                                        --- LOGIC FOR ORDER MATCH -

                                                        SELECT SYSDATE INTO CURRENTDATE FROM DUAL;
                                                        --DELETE FROM TEMP1;


                                                        SELECT  TEMPSTGTESTCASE_SEQ.NEXTVAL INTO AUTO_TEMPSTGTESTCASE FROM DUAL;

                                                        --INSERT INTO TEMPSTGTESTCASE(TESTCASENAME,DATASETNAME,KEYWORDNAME,OBJECTNAME,ORDERNUMBER) 
                                                        --SELECT DISTINCT TESTCASENAME,DATASETNAME,KEYWORD,OBJECT,ROWNUMBER FROM TBLSTGTESTCASE WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;


                                                        SELECT  TEMPDWTESTCASE_SEQ.NEXTVAL INTO AUTO_TEMPDWTESTCASE FROM DUAL;

                                                        --INSERT INTO TEMPDWTESTCASE(TESTCASENAME,DATASETNAME,KEYWORDNAME,OBJECTNAME,ORDERNUMBER) 
                                                        --SELECT DISTINCT
                                                            --T_TEST_CASE_SUMMARY.TEST_CASE_NAME,
                                                            --DATASETNAME,T_KEYWORD.KEY_WORD_NAME,TBLSTGTESTCASE.OBJECT,TBLSTGTESTCASE.ROWNUMBER 
                                                        --FROM TBLSTGTESTCASE
                                                        --INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(TBLSTGTESTCASE.TESTCASENAME)
                                                        --INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME)  =UPPER(TBLSTGTESTCASE.KEYWORD)
                                                        --WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;




                                                        SELECT  TEMP1_SEQ.NEXTVAL INTO AUTO_TEMP1 FROM DUAL;

                                                        --INSERT INTO TEMP1 (TESTCASENAME,DATASETNAME)
                                                        --SELECT DISTINCT TEMPSTGTESTCASE.TESTCASENAME,TEMPSTGTESTCASE.DATASETNAME
                                                        --FROM TEMPSTGTESTCASE
                                                        --WHERE NOT EXISTS(SELECT TEMPDWTESTCASE.TESTCASENAME,TEMPDWTESTCASE.DATASETNAME 
                                                        --FROM TEMPDWTESTCASE 
                                                        --WHERE UPPER(TEMPDWTESTCASE.TESTCASENAME) =UPPER(TEMPSTGTESTCASE.TESTCASENAME)
                                                        --AND UPPER(TEMPDWTESTCASE.DATASETNAME)=UPPER(TEMPSTGTESTCASE.DATASETNAME)
                                                        --AND UPPER(TEMPDWTESTCASE.KEYWORDNAME)=UPPER(TEMPSTGTESTCASE.KEYWORDNAME)
                                                        ----AND UPPER(TEMPDWTESTCASE.OBJECTNAME)=UPPER(TEMPSTGTESTCASE.OBJECTNAME)
                                                        --AND TEMPDWTESTCASE.ORDERNUMBER = TEMPSTGTESTCASE.ORDERNUMBER
                                                        --);

                                      DECLARE 
                                                MAXRow INT;
                                                OBJECT_ID INT;
                                                OBJECT_ID_Set INT;
                                                AUTO_LOGREPORT INT;
                                                ERROR VARCHAR2(500);
                                                MESSAGE VARCHAR2(500);
                                                MESSAGESYMBOL VARCHAR2(500);
                                                CURRENTTIME TIMESTAMP;
                                                COUNT_T_TEST_STEPS INT:=0;
                                                COUNT_T_SHARED_OBJECT_POOL INT:=0;
                                                COUNT_TEST_DATA_SETTING INT:=0;
                                      BEGIN    
                                                dbms_output.put_line('Log Insert Started');
                                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;
                                                dbms_output.put_line(CURRENTTIME);
                                            -- Insert All Log Reports - Start  
                                            -- Insert log for the application not found for the tab.
                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=18;
                                                SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=18;

                                                INSERT INTO "TBLLOGREPORT"
                                                (
                                                    CREATIONDATE,
                                                    MESSAGETYPE,
                                                    ACTION,
                                                    CELLADDRESS,
                                                    VALIDATIONNAME,
                                                    VALIDATIONDESCRIPTION,
                                                    APPLICATIONNAME,
                                                    TESTCASENAME,
                                                    DATASETNAME,
                                                    TESTSTEPNUMBER,
                                                    OBJECTNAME,
                                                    COMMENTDATA,
                                                    ERRORDESCRIPTION,
                                                    PROGRAMLOCATION,
                                                    FEEDPROCESSINGDETAILSID,
                                                    FEEDPROCESSID
                                                )
                                                SELECT DISTINCT
                                                    (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                    VALIDATETYPE,
                                                    'Validation',
                                                    '',
                                                    VALIDATETYPE,
                                                    ERROR,
                                                    APPLICATION,
                                                    TESTCASENAME,
                                                    '',
                                                    '',
                                                    '',
                                                    '',
                                                    ERROR,
                                                    '',
                                                    I_INDEX.FEEDPROCESSDETAILID,
                                                    FEEDPROCESSID1
                                                FROM (
                                                    SELECT DISTINCT 
                                                        tblstgtestcase.TESTCASENAME,
                                                        tblstgtestcase.DATASETNAME ,
                                                        tblstgtestcase.APPLICATION
                                                    FROM tblstgtestcase
                                                    INNER JOIN TEMPFALSEAPPLICATION ON TEMPFALSEAPPLICATION.TESTSUITNAME = tblstgtestcase.testsuitename
                                                        AND TEMPFALSEAPPLICATION.TESTCASENAME = tblstgtestcase.testcasename
                                                        AND tempfalseapplication.flag = 0
                                                    WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                ) x;    


                                            -- Insert log for the application not found for the tab.

                                            -- Insert log for Multiple Test Suite names - Start
                                              DECLARE 
                                                CountOfTestSuites NUMERIC:=0;
                                              BEGIN

                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=28;
                                                SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=28;

                                                SELECT COUNT(DISTINCT TESTSUITENAME) INTO CountOfTestSuites
                                                FROM tblstgtestcase
                                                WHERE feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

                                                IF CountOfTestSuites > 1
                                                THEN
                                                BEGIN
                                                  INSERT INTO "TBLLOGREPORT"
                                                  (
                                                      CREATIONDATE,
                                                      MESSAGETYPE,
                                                      ACTION,
                                                      CELLADDRESS,
                                                      VALIDATIONNAME,
                                                      VALIDATIONDESCRIPTION,
                                                      APPLICATIONNAME,
                                                      TESTCASENAME,
                                                      DATASETNAME,
                                                      TESTSTEPNUMBER,
                                                      OBJECTNAME,
                                                      COMMENTDATA,
                                                      ERRORDESCRIPTION,
                                                      PROGRAMLOCATION,
                                                      FEEDPROCESSINGDETAILSID,
                                                      FEEDPROCESSID
                                                  )
                                                  SELECT DISTINCT
                                                      (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                      VALIDATETYPE,
                                                      'Validation',
                                                      '',
                                                      VALIDATETYPE,
                                                      ERROR,
                                                      APPLICATION,
                                                      '',
                                                      '',
                                                      '',
                                                      '',
                                                      '',
                                                      ERROR,
                                                      '',
                                                      I_INDEX.FEEDPROCESSDETAILID,
                                                      FEEDPROCESSID1
                                                  FROM tblstgtestcase
                                                  WHERE feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;
                                                END;
                                                END IF;

                                              END;
                                              -- Insert log for Multiple Test Suite names - End

                                            -- Inser log for Keyword not found - Start
                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=1;
                                                SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=1;


                                                INSERT INTO "TBLLOGREPORT"
                                                (
                                                    CREATIONDATE,
                                                    MESSAGETYPE,
                                                    ACTION,
                                                    CELLADDRESS,
                                                    VALIDATIONNAME,
                                                    VALIDATIONDESCRIPTION,
                                                    APPLICATIONNAME,
                                                    TESTCASENAME,
                                                    DATASETNAME,
                                                    TESTSTEPNUMBER,
                                                    OBJECTNAME,
                                                    COMMENTDATA,
                                                    ERRORDESCRIPTION,
                                                    PROGRAMLOCATION,
                                                    FEEDPROCESSINGDETAILSID,
                                                    FEEDPROCESSID
                                                )
                                                SELECT DISTINCT
                                                    (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                    VALIDATETYPE,
                                                    'Validation',
                                                    '',
                                                    VALIDATETYPE,
                                                    ERROR,
                                                    APPLICATION,
                                                    TESTCASENAME,
                                                    '',
                                                    ROWNUMBER,
                                                    '',
                                                    Keyword,
                                                    ERROR,
                                                    KEYWORD,
                                                    I_INDEX.FEEDPROCESSDETAILID,
                                                    FEEDPROCESSID1
                                                FROM (
                                                    SELECT DISTINCT 
                                                        tblstgtestcase.TESTCASENAME,
                                                        tblstgtestcase.DATASETNAME ,
                                                        tblstgtestcase.APPLICATION,
                                                        tblstgtestcase.ROWNUMBER,
                                                        tblstgtestcase.KEYWORD
                                                    FROM tblstgtestcase
                                                    WHERE tblstgtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                     and tblstgtestcase.KEYWORD IS NOT NULL 
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM t_keyword 
                                                            WHERE UPPER(t_keyword.key_word_name) = UPPER(tblstgtestcase.KEYWORD)
                                                            AND ROWNUM=1
                                                        )
                                                        /*
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM tempfalseapplication
                                                            where tempfalseapplication.testcasename = TBLSTGTESTCASE.TESTCASENAME
                                                                AND tempfalseapplication.FLAG = 0
                                                                AND ROWNUM=1
                                                        )
                                                        */
                                                );        

                                            -- Inser log for Keyword not found - End

                                            -- Insert log for Duplicate DataSets - Start
                                            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=27;
                                            SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=27;

                                            INSERT INTO "TBLLOGREPORT"
                                            (
                                                CREATIONDATE,
                                                MESSAGETYPE,
                                                ACTION,
                                                CELLADDRESS,
                                                VALIDATIONNAME,
                                                VALIDATIONDESCRIPTION,
                                                APPLICATIONNAME,
                                                TESTCASENAME,
                                                DATASETNAME,
                                                TESTSTEPNUMBER,
                                                OBJECTNAME,
                                                COMMENTDATA,
                                                ERRORDESCRIPTION,
                                                PROGRAMLOCATION,
                                                FEEDPROCESSINGDETAILSID,
                                                FEEDPROCESSID
                                            )
                                            SELECT DISTINCT
                                              SYSDATE,
                                              VALIDATETYPE,
                                              'Validation',
                                              '',
                                              VALIDATETYPE,
                                              ERROR,
                                              APPLICATION,
                                              TESTCASENAME,
                                              DATASETNAME,
                                              '',
                                              '',
                                              '',
                                              ERROR,
                                              '',
                                              I_INDEX.FEEDPROCESSDETAILID,
                                              FEEDPROCESSID1
                                            FROM (  
                                              SELECT NVL(OBJECT, 'N/A'), APPLICATION,KEYWORD,DATASETNAME, TESTCASENAME, ROWNUMBER,COUNT(1)
                                              FROM TBLSTGTESTCASE
                                              WHERE FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                              GROUP BY NVL(OBJECT, 'N/A'), KEYWORD,DATASETNAME, TESTCASENAME, ROWNUMBER, APPLICATION
                                              HAVING COUNT(1) > 1
                                              ORDER BY TESTCASENAME, ROWNUMBER
                                            ) xy;
                                            -- Insert log for Duplicate DataSets - End

                                            -- Insert log for count of steps in spreadsheet and count of steps in warehouse does not match - start

                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=7;
                                                SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=7;

                                                INSERT INTO "TBLLOGREPORT"
                                                (
                                                  CREATIONDATE,
                                                  MESSAGETYPE,
                                                  ACTION,
                                                  CELLADDRESS,
                                                  VALIDATIONNAME,
                                                  VALIDATIONDESCRIPTION,
                                                  APPLICATIONNAME,
                                                  TESTCASENAME,
                                                  DATASETNAME,
                                                  TESTSTEPNUMBER,
                                                  OBJECTNAME,
                                                  COMMENTDATA,
                                                  ERRORDESCRIPTION,
                                                  PROGRAMLOCATION,
                                                  FEEDPROCESSINGDETAILSID,
                                                  FEEDPROCESSID
                                                )
                                                SELECT DISTINCT
                                                    (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                    VALIDATETYPE,
                                                    'Validation',
                                                    '',
                                                    VALIDATETYPE,
                                                    ERROR,
                                                    APPLICATION,
                                                    TESTCASENAME,
                                                    '',
                                                    '',
                                                    '',
                                                    '',
                                                    ERROR,
                                                    '',
                                                    I_INDEX.FEEDPROCESSDETAILID,
                                                    FEEDPROCESSID1
                                                FROM (
                                                    SELECT DISTINCT 
                                                        X.TESTCASENAME,
                                                        X.DATASETNAME,
                                                        X.APPLICATION
                                                    FROM (
                                                        SELECT TESTCASENAME,DATASETNAME,MAX(ROWNUMBER) as CountofSteps,APPLICATION
                                                        FROM TBLSTGTESTCASE
                                                        WHERE KEYWORD IS NOT NULL
                                                            AND FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                            AND DATASETMODE IS NULL
                                                            /*
                                                            AND NOT EXISTS (
                                                                SELECT 1
                                                                FROM TEMPFALSEAPPLICATION
                                                                WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                    AND tempfalseapplication.FLAG = 0
                                                                    AND ROWNUM=1
                                                            )
                                                            */
                                                        GROUP BY TESTCASENAME,DATASETNAME,APPLICATION
                                                        ORDER BY 1,3
                                                    ) x    
                                                    INNER JOIN 
                                                    (
                                                        SELECT tblstgtestcase.TESTCASENAME, tblstgtestcase.DATASETNAME, MAX(RUN_ORDER) as CountofSteps, tblstgtestcase.Application
                                                        FROM tblstgtestcase
                                                        INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.TESTCASENAME)
                                                        INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.DATASETNAME)
                                                        INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgtestcase.KEYWORD)
                                                        INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.TEST_CASE_ID
                                                            AND T_TEST_STEPS.KEY_WORD_ID = T_KEYWORD.KEY_WORD_ID
                                                            AND T_TEST_STEPS.RUN_ORDER = tblstgtestcase.ROWNUMBER
                                                            AND T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                        WHERE tblstgtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        /*
                                                            AND NOT EXISTS (
                                                                SELECT 1
                                                                FROM TEMPFALSEAPPLICATION
                                                                WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                    AND tempfalseapplication.FLAG = 0
                                                                    AND ROWNUM=1
                                                            )
                                                            */
                                                        GROUP BY tblstgtestcase.TESTCASENAME, tblstgtestcase.DATASETNAME,tblstgtestcase.Application
                                                        ORDER BY 1,3
                                                    ) y
                                                    ON x.TESTCASENAME = y.TESTCASENAME
                                                        AND x.DATASETNAME = y.DATASETNAME
                                                        AND x.CountofSteps <> y.CountofSteps    
                                                ) xyz;        
                                            -- Insert log for count of dataset in spreadsheet and count of datasets in warehouse does not match - end


                                            /*
                                            -- Insert log for records where Order of test case not matched - start

                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=13;
                                                SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=13;

                                                INSERT INTO "TBLLOGREPORT"(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,APPLICATIONNAME,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
                                                SELECT 
                                                    (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                    VALIDATETYPE,
                                                    'Validation',
                                                    '',
                                                    'STEPS ID NOT FOUND',
                                                    ERROR,
                                                    APPLICATION,
                                                    TESTCASENAME,
                                                    DATASETNAME,
                                                    rownumber,
                                                    '',
                                                    '',
                                                    ERROR,
                                                    '',
                                                    I_INDEX.FEEDPROCESSDETAILID,
                                                    FEEDPROCESSID1
                                                FROM (
                                                    SELECT DISTINCT 
                                                        tblstgtestcase.TESTCASENAME,
                                                        tblstgtestcase.DATASETNAME ,
                                                        tblstgtestcase.APPLICATION,
                                                        tblstgtestcase.rownumber
                                                    FROM tblstgtestcase
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                                    INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                                        AND tblstgtestcase.datasetname IS NOT NULL
                                                         and tblstgtestcase.KEYWORD IS NOT NULL 
                                                    INNER JOIN T_KEYWORD ON T_KEYWORD.KEY_WORD_NAME = tblstgtestcase.KEYWORD
                                                    CROSS JOIN TABLE(GETTOPOBJECT(tblstgtestcase.object)) t
                                                    WHERE tblstgtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND tblstgtestcase.datasetvalue IS NOT NULL
                                                        AND tblstgtestcase.datasetmode IS NULL
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM TEMPFALSEAPPLICATION
                                                            WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                AND tempfalseapplication.FLAG = 0
                                                                AND ROWNUM=1
                                                        )
                                                        AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM t_test_steps
                                                            WHERE t_test_steps.key_word_id = t_keyword.key_word_id
                                                                AND t_test_steps.object_id = t.OBJECT_ID
                                                                AND t_test_steps.test_case_id = T_TEST_CASE_SUMMARY.test_case_id
                                                                AND t_test_steps.run_order = tblstgtestcase.rownumber
                                                                AND ROWNUM=1
                                                        )
                                                );
                                            -- Insert log for records where Order of test case not matched - end
                                            */

                                            -- Insert log for records where Order of test case not matched - start
                                            /*
                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=8;
                                                SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=8;

                                                INSERT INTO "TBLLOGREPORT"
                                                (
                                                    CREATIONDATE,
                                                    MESSAGETYPE,
                                                    ACTION,
                                                    CELLADDRESS,
                                                    VALIDATIONNAME,
                                                    VALIDATIONDESCRIPTION,
                                                    APPLICATIONNAME,
                                                    TESTCASENAME,
                                                    DATASETNAME,
                                                    TESTSTEPNUMBER,
                                                    OBJECTNAME,
                                                    COMMENTDATA,
                                                    ERRORDESCRIPTION,
                                                    PROGRAMLOCATION,
                                                    FEEDPROCESSINGDETAILSID,
                                                    FEEDPROCESSID
                                                )
                                                SELECT DISTINCT
                                                    (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                    VALIDATETYPE,
                                                    'Validation',
                                                    '',
                                                    VALIDATETYPE,
                                                    ERROR,
                                                    APPLICATION,
                                                    TESTCASENAME,
                                                    '',
                                                    rownumber,
                                                    '',
                                                    '',
                                                    ERROR,
                                                    '',
                                                    I_INDEX.FEEDPROCESSDETAILID,
                                                    FEEDPROCESSID1
                                                FROM (
                                                    SELECT DISTINCT tblstgtestcase.TESTCASENAME,tblstgtestcase.DATASETNAME ,tblstgtestcase.APPLICATION,tblstgtestcase.rownumber
                                                    FROM tblstgtestcase
                                                    INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.testcasename)
                                                    INNER JOIN T_TEST_DATA_SUMMARY ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.datasetname)
                                                        AND tblstgtestcase.datasetname IS NOT NULL
                                                         and tblstgtestcase.KEYWORD IS NOT NULL 
                                                    INNER JOIN T_KEYWORD ON T_KEYWORD.KEY_WORD_NAME = tblstgtestcase.KEYWORD
                                                    -- GETTOPOBJECT REPLACE START
                                                    LEFT JOIN T_OBJECT_NAMEINFO ON T_OBJECT_NAMEINFO.OBJECT_HAPPY_NAME = tblstgtestcase.object
                                                    LEFT JOIN T_REGISTED_OBJECT ON T_REGISTED_OBJECT.OBJECT_NAME_ID = T_OBJECT_NAMEINFO.OBJECT_NAME_ID
                                                      AND T_REGISTED_OBJECT.APPLICATION_ID = APPLICATIONID
                                                    --CROSS JOIN TABLE(GETTOPOBJECT(tblstgtestcase.object, APPLICATIONID)) t
                                                    -- GETTOPOBJECT REPLACE END
                                                    WHERE tblstgtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        AND tblstgtestcase.datasetvalue IS NOT NULL
                                                        AND tblstgtestcase.datasetmode IS NULL
                                                        
                                                        //AND NOT EXISTS (
                                                        //    SELECT 1
                                                        //    FROM TEMPFALSEAPPLICATION
                                                        //    WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                        //        AND tempfalseapplication.FLAG = 0
                                                        //        AND ROWNUM=1
                                                        //)
                                                        AND (tblstgtestcase.OBJECT IS NOT NULL AND NOT EXISTS (
                                                            SELECT 1
                                                            FROM t_test_steps
                                                            WHERE t_test_steps.key_word_id = t_keyword.key_word_id
                                                                AND t_test_steps.object_name_id = NVL(t_object_nameinfo.object_name_id, 0)
                                                                --cherish
                                                                --AND t_test_steps.object_id = NVL(T_REGISTED_OBJECT.OBJECT_ID, 0)
                                                                AND t_test_steps.test_case_id = T_TEST_CASE_SUMMARY.test_case_id
                                                                AND ROWNUM=1
                                                                --AND t_test_steps.run_order = tblstgtestcase.rownumber
                                                        ))
                                                );
                                            */    
                                            -- Insert log for records where Order of test case not matched - end


                                            -- Insert All Log Reports - End


                                            -- Inser Log for Object and Keyword Linking Start

                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=22;
                                                SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=22;

                                                INSERT INTO "TBLLOGREPORT"
                                                (
                                                    CREATIONDATE,
                                                    MESSAGETYPE,
                                                    ACTION,
                                                    CELLADDRESS,
                                                    VALIDATIONNAME,
                                                    VALIDATIONDESCRIPTION,
                                                    APPLICATIONNAME,
                                                    TESTCASENAME,
                                                    DATASETNAME,
                                                    TESTSTEPNUMBER,
                                                    OBJECTNAME,
                                                    COMMENTDATA,
                                                    ERRORDESCRIPTION,
                                                    PROGRAMLOCATION,
                                                    FEEDPROCESSINGDETAILSID,
                                                    FEEDPROCESSID
                                                )
                                                SELECT DISTINCT
                                                  (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                  VALIDATETYPE,
                                                  'Validation',
                                                  '',
                                                  VALIDATETYPE,
                                                  ERROR,
                                                  stg.APPLICATION,
                                                  stg.TESTCASENAME,
                                                  '',
                                                  stg.rownumber,
                                                  stg.object,
                                                  stg.comments,
                                                  ERROR,
                                                  '',
                                                  I_INDEX.FEEDPROCESSDETAILID,
                                                  FEEDPROCESSID1
                                                FROM TBLSTGTESTCASE stg
                                                INNER JOIN T_KEYWORD t ON t.KEY_WORD_NAME = stg.KEYWORD
                                                INNER JOIN T_DIC_RELATION_KEYWORD tdic ON tdic.KEY_WORD_ID = t.KEY_WORD_ID
                                                  AND NVL(tdic.TYPE_ID, 0) <> 0
                                                WHERE stg.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                  AND NVL(stg.KEYWORD, 'N/A') = 'N/A'
                                                UNION
                                                SELECT DISTINCT
                                                  (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                  VALIDATETYPE,
                                                  'Validation',
                                                  '',
                                                  VALIDATETYPE,
                                                  ERROR,
                                                  stg.APPLICATION,
                                                  stg.TESTCASENAME,
                                                  '',
                                                  stg.rownumber,
                                                  stg.object,
                                                  stg.comments,
                                                  ERROR,
                                                  '',
                                                  I_INDEX.FEEDPROCESSDETAILID,
                                                  FEEDPROCESSID1
                                                FROM TBLSTGTESTCASE stg
                                                INNER JOIN T_OBJECT_NAMEINFO tobjn ON tobjn.OBJECT_HAPPY_NAME = stg.OBJECT
                                                INNER JOIN T_REGISTED_OBJECT treg ON treg.OBJECT_NAME_ID = tobjn.OBJECT_NAME_ID
                                                WHERE stg.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                  AND EXISTS (
                                                    SELECT 1
                                                    FROM T_KEYWORD tk
                                                    INNER JOIN T_DIC_RELATION_KEYWORD tdic ON tdic.KEY_WORD_ID = tk.KEY_WORD_ID
                                                    WHERE tk.KEY_WORD_NAME = stg.KEYWORD
                                                  )
                                                  AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM T_KEYWORD tk
                                                    INNER JOIN T_DIC_RELATION_KEYWORD tdic ON tdic.KEY_WORD_ID = tk.KEY_WORD_ID
                                                    WHERE tk.KEY_WORD_NAME = stg.KEYWORD
                                                      AND tdic.TYPE_ID = treg.TYPE_ID
                                                  );

                                            -- Insert Log for Object and Keyword Linking End        


                                            -- Insert Log for Object application linking Start
                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=26;
                                                SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=26;
                                                INSERT INTO "TBLLOGREPORT"
                                                (
                                                    CREATIONDATE,
                                                    MESSAGETYPE,
                                                    ACTION,
                                                    CELLADDRESS,
                                                    VALIDATIONNAME,
                                                    VALIDATIONDESCRIPTION,
                                                    APPLICATIONNAME,
                                                    TESTCASENAME,
                                                    DATASETNAME,
                                                    TESTSTEPNUMBER,
                                                    OBJECTNAME,
                                                    COMMENTDATA,
                                                    ERRORDESCRIPTION,
                                                    PROGRAMLOCATION,
                                                    FEEDPROCESSINGDETAILSID,
                                                    FEEDPROCESSID
                                                )
                                                SELECT DISTINCT
                                                    (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                    VALIDATETYPE,
                                                    'Validation',
                                                    '',
                                                    VALIDATETYPE,
                                                    ERROR,
                                                    t.APPLICATION,
                                                    t.TESTCASENAME,
                                                    '',
                                                    t.rownumber,
                                                    t.object,
                                                    t.comments,
                                                    ERROR,
                                                    '',
                                                    I_INDEX.FEEDPROCESSDETAILID,
                                                    FEEDPROCESSID1
                                                FROM tblstgtestcase t
                                                INNER JOIN TEMPFALSEAPPLICATION f ON f.TESTCASENAME = t.TESTCASENAME
                                                INNER JOIN T_REGISTERED_APPS a ON a.app_short_name = f.application
                                                WHERE feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                    AND NVL(DATASETMODE, 'N/A') = 'N/A'
                                                    AND NVL(OBJECT, 'N/A') <> 'N/A'
                                                    AND DATASETMODE IS NULL
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM t_object_nameinfo tn
                                                        INNER JOIN t_registed_object tro ON tro.object_name_id = tn.object_name_id
                                                        WHERE tn.object_happy_name = t.object
                                                            AND tro.application_id = a.application_id
                                                    );
                                            -- Insert Log for Object application linking End

                                            -- INSERT LOG FOR PEG WINDOW VALIDATION - START

                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=21;
                                                SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=21;

                                                INSERT INTO "TBLLOGREPORT"
                                                (
                                                    CREATIONDATE,
                                                    MESSAGETYPE,
                                                    ACTION,
                                                    CELLADDRESS,
                                                    VALIDATIONNAME,
                                                    VALIDATIONDESCRIPTION,
                                                    APPLICATIONNAME,
                                                    TESTCASENAME,
                                                    DATASETNAME,
                                                    TESTSTEPNUMBER,
                                                    OBJECTNAME,
                                                    COMMENTDATA,
                                                    ERRORDESCRIPTION,
                                                    PROGRAMLOCATION,
                                                    FEEDPROCESSINGDETAILSID,
                                                    FEEDPROCESSID
                                                )
                                                SELECT 
                                                    (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                    VALIDATETYPE,
                                                    'Validation',
                                                    '',
                                                    VALIDATETYPE,
                                                    ERROR,
                                                    t.APPLICATION,
                                                    t.TESTCASENAME,
                                                    t.DATASETNAME,
                                                    t.rownumber,
                                                    t.object,
                                                    '',
                                                    ERROR,
                                                    '',
                                                    I_INDEX.FEEDPROCESSDETAILID,
                                                    FEEDPROCESSID1
                                                FROM tblstgtestcase t
                                                INNER JOIN t_object_nameinfo p1 ON p1.object_happy_name = t.OBJECT
                                                WHERE  t.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID 
                                                    AND t.KEYWORD IS NOT NULL 
                                                    AND t.DATASETMODE IS NULL
                                                    AND t.object IS NOT NULL
                                                    -- check immediate parent must be of PegWindow
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM t_object_nameinfo ton
                                                        INNER JOIN t_registed_object o ON o.object_name_id = ton.object_name_id
                                                        INNER JOIN t_object_nameinfo p ON p.object_happy_name = o.object_type
                                                        INNER JOIN t_registed_object op ON op.object_name_id = p.object_name_id
                                                        INNER JOIN T_GUI_COMPONENT_TYPE_DIC tg ON tg.TYPE_ID = op.TYPE_ID
                                                          AND LOWER(tg.TYPE_NAME) = 'pegwindow'
                                                        WHERE ton.OBJECT_HAPPY_NAME = t.object
                                                        --and t.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID and rownum=1

                                                    )
                                                    -- check if current is of PegWindow
                                                    AND NOT EXISTS
                                                    (
                                                        SELECT  1
                                                        FROM t_registed_object o
                                                        INNER JOIN T_GUI_COMPONENT_TYPE_DIC tg ON tg.TYPE_ID = o.TYPE_ID
                                                          AND LOWER(tg.TYPE_NAME) = 'pegwindow'
                                                        WHERE o.OBJECT_NAME_ID = p1.OBJECT_NAME_ID
                                                    );

                                            -- INSERT LOG FOR PEG WINDOW VALIDATION - End

                                            -- Insert Log for Peg Window Order in Spreadsheet - Start

                                            -- Insert Log for Peg Window Order in Spreadsheet - End

                                            -- IF ELSE  Order in Spreadsheet - Start
                                            DECLARE
                                              IFCOUNT INT:=0;
                                              ELSECOUNT INT:=0;
                                              IFENDCOUNT INT:=0;
                                            BEGIN  

                                              EXECUTE IMMEDIATE 'DELETE FROM IFORDER';
                                              EXECUTE IMMEDIATE 'DELETE FROM ELSEORDER';
                                              EXECUTE IMMEDIATE 'DELETE FROM IFENDORDER';
                                              EXECUTE IMMEDIATE 'DELETE FROM PEGWINODWORDER';

                                              --==========================================
                                              -- Insert IF steps and update them with Adjusant Pegwindow
                                              --==========================================
                                              INSERT INTO IFORDER
                                              (
                                                ID,
                                                IFSTEPID,
                                                PEGWINDOWSTEPID,
                                                TESTCASENAME,
                                                FEEDPROCESSDETAILSID
                                              )
                                              SELECT 
                                                0 + ROWNUM,
                                                ROWNUMBER,
                                                NULL,
                                                TESTCASENAME,
                                                FEEDPROCESSDETAILID
                                              FROM (  
                                                    SELECT DISTINCT t.ROWNUMBER, t.TESTCASENAME, t.FEEDPROCESSDETAILID
                                                    FROM tblstgtestcase t
                                                    WHERE t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                    AND t.KEYWORD = 'IF'
                                              ) xy;

                                              -- select * from IFORDER;
                                              -- select * from tblfeedprocessdetails where feedprocessdetailid = 943
                                              MERGE INTO IFORDER ifstp
                                              USING (
                                                      SELECT DISTINCT ifo.IFSTEPID, t.ROWNUMBER, ifo.ID
                                                      FROM tblstgtestcase t
                                                      INNER JOIN IFORDER ifo ON ifo.FEEDPROCESSDETAILSID = t.FEEDPROCESSDETAILID
                                                        AND ifo.TESTCASENAME = t.TESTCASENAME
                                                        AND ifo.IFSTEPID > t.ROWNUMBER
                                                        AND t.ROWNUMBER = (
                                                          SELECT MAX(x.ROWNUMBER)
                                                          FROM tblstgtestcase x
                                                          WHERE ifo.FEEDPROCESSDETAILSID = x.FEEDPROCESSDETAILID
                                                            AND ifo.TESTCASENAME = x.TESTCASENAME
                                                            AND ifo.IFSTEPID > x.ROWNUMBER
                                                            AND UPPER(x.KEYWORD) = 'PEGWINDOW'
                                                        )
                                                      WHERE t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                        AND UPPER(t.KEYWORD) = 'PEGWINDOW'
                                              ) TMP
                                              ON (TMP.ID = ifstp.ID)
                                              WHEN MATCHED THEN
                                                UPDATE SET
                                                  ifstp.PEGWINDOWSTEPID = TMP.ROWNUMBER;
                                              dbms_output.put_line('iforderdone');
                                              --==========================================

                                              --==========================================
                                              -- Insert ELSE steps and update them with Adjusant Pegwindow
                                              --==========================================
                                              INSERT INTO ELSEORDER
                                              (
                                                ID,
                                                ELSESTEPID,
                                                PEGWINDOWSTEPID,
                                                TESTCASENAME,
                                                FEEDPROCESSDETAILSID
                                              )
                                              SELECT 
                                                0 + ROWNUM,
                                                ROWNUMBER,
                                                NULL,
                                                TESTCASENAME,
                                                FEEDPROCESSDETAILID
                                              FROM (  
                                                    SELECT DISTINCT t.ROWNUMBER, t.TESTCASENAME, t.FEEDPROCESSDETAILID
                                                    FROM tblstgtestcase t
                                                    WHERE t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                    AND t.KEYWORD = 'ELSE'
                                              ) xy;


                                              MERGE INTO ELSEORDER ifstp
                                              USING (
                                                      SELECT DISTINCT ifo.ELSESTEPID, t.ROWNUMBER, ifo.ID
                                                      FROM tblstgtestcase t
                                                      INNER JOIN ELSEORDER ifo ON ifo.FEEDPROCESSDETAILSID = t.FEEDPROCESSDETAILID
                                                        AND ifo.TESTCASENAME = t.TESTCASENAME
                                                        AND ifo.ELSESTEPID > t.ROWNUMBER
                                                        AND t.ROWNUMBER = (
                                                          SELECT MAX(x.ROWNUMBER)
                                                          FROM tblstgtestcase x
                                                          WHERE ifo.FEEDPROCESSDETAILSID = x.FEEDPROCESSDETAILID
                                                            AND ifo.TESTCASENAME = x.TESTCASENAME
                                                            AND ifo.ELSESTEPID > x.ROWNUMBER
                                                            AND UPPER(x.KEYWORD) = 'PEGWINDOW'
                                                        )
                                                      WHERE t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                        AND UPPER(t.KEYWORD) = 'PEGWINDOW'
                                              ) TMP
                                              ON (TMP.ID = ifstp.ID)
                                              WHEN MATCHED THEN
                                                UPDATE SET
                                                  ifstp.PEGWINDOWSTEPID = TMP.ROWNUMBER;
                                              dbms_output.put_line('elseorderdone');
                                              --==========================================

                                              --==========================================
                                              -- Insert IFEND steps and update them with Adjusant Pegwindow
                                              --==========================================
                                              INSERT INTO IFENDORDER
                                              (
                                                ID,
                                                IFENDSTEPID,
                                                PEGWINDOWSTEPID,
                                                TESTCASENAME,
                                                FEEDPROCESSDETAILSID
                                              )
                                              SELECT 
                                                0 + ROWNUM,
                                                ROWNUMBER,
                                                NULL,
                                                TESTCASENAME,
                                                FEEDPROCESSDETAILID
                                              FROM (  
                                                    SELECT DISTINCT t.ROWNUMBER, t.TESTCASENAME, t.FEEDPROCESSDETAILID
                                                    FROM tblstgtestcase t
                                                    WHERE t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                    AND t.KEYWORD = 'IFEND'
                                              ) xy;


                                              MERGE INTO IFENDORDER ifstp
                                              USING (
                                                      SELECT DISTINCT ifo.IFENDSTEPID, t.ROWNUMBER, ifo.ID
                                                      FROM tblstgtestcase t
                                                      INNER JOIN IFENDORDER ifo ON ifo.FEEDPROCESSDETAILSID = t.FEEDPROCESSDETAILID
                                                        AND ifo.TESTCASENAME = t.TESTCASENAME
                                                        AND ifo.IFENDSTEPID > t.ROWNUMBER
                                                        AND t.ROWNUMBER = (
                                                          SELECT MAX(x.ROWNUMBER)
                                                          FROM tblstgtestcase x
                                                          WHERE ifo.FEEDPROCESSDETAILSID = x.FEEDPROCESSDETAILID
                                                            AND ifo.TESTCASENAME = x.TESTCASENAME
                                                            AND ifo.IFENDSTEPID > x.ROWNUMBER
                                                            AND UPPER(x.KEYWORD) = 'PEGWINDOW'
                                                        )
                                                      WHERE t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                        AND UPPER(t.KEYWORD) = 'PEGWINDOW'
                                              ) TMP
                                              ON (TMP.ID = ifstp.ID)
                                              WHEN MATCHED THEN
                                                UPDATE SET
                                                  ifstp.PEGWINDOWSTEPID = TMP.ROWNUMBER;

                                              dbms_output.put_line('ifendorderdone');

                                              INSERT INTO PEGWINODWORDER
                                              (
                                                ID,
                                                PEGWINDOW,
                                                TESTCASENAME,
                                                FEEDPROCESSDETAILSID
                                              )
                                              SELECT 
                                                0 + ROWNUM,
                                                0, TESTCASENAME, FEEDPROCESSDETAILSID
                                              FROM (
                                                SELECT DISTINCT PEGWINDOWSTEPID, TESTCASENAME, FEEDPROCESSDETAILSID
                                                FROM IFORDER
                                                WHERE FEEDPROCESSDETAILSID = FEEDPROCESSDETAILSID
                                                UNION
                                                SELECT DISTINCT PEGWINDOWSTEPID, TESTCASENAME, FEEDPROCESSDETAILSID
                                                FROM ELSEORDER
                                                WHERE FEEDPROCESSDETAILSID = FEEDPROCESSDETAILSID
                                                UNION
                                                SELECT DISTINCT PEGWINDOWSTEPID, TESTCASENAME, FEEDPROCESSDETAILSID
                                                FROM IFENDORDER
                                                WHERE FEEDPROCESSDETAILSID = FEEDPROCESSDETAILSID
                                              ) xy
                                              WHERE NOT EXISTS (
                                                SELECT 1
                                                FROM PEGWINODWORDER x
                                                WHERE x.PEGWINDOW = xy.PEGWINDOWSTEPID
                                                  AND x.TESTCASENAME = xy.TESTCASENAME
                                                  AND x.FEEDPROCESSDETAILSID = xy.FEEDPROCESSDETAILSID
                                              );

                                              dbms_output.put_line('pegwindoworderdone');

                                              SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=25;
                                              SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=25;

                                              INSERT INTO "TBLLOGREPORT"
                                              (
                                                  CREATIONDATE,
                                                  MESSAGETYPE,
                                                  ACTION,
                                                  CELLADDRESS,
                                                  VALIDATIONNAME,
                                                  VALIDATIONDESCRIPTION,
                                                  APPLICATIONNAME,
                                                  TESTCASENAME,
                                                  DATASETNAME,
                                                  TESTSTEPNUMBER,
                                                  OBJECTNAME,
                                                  COMMENTDATA,
                                                  ERRORDESCRIPTION,
                                                  PROGRAMLOCATION,
                                                  FEEDPROCESSINGDETAILSID,
                                                  FEEDPROCESSID
                                              )
                                              SELECT DISTINCT
                                                (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                VALIDATETYPE,
                                                'Validation',
                                                '',
                                                VALIDATETYPE,
                                                ERROR,
                                                t.APPLICATION,
                                                t.TESTCASENAME,
                                                '',
                                                '',
                                                '',
                                                '',
                                                ERROR,
                                                '',
                                                fdp.FEEDPROCESSDETAILID,
                                                fdp.FEEDPROCESSID
                                              FROM (
                                                  SELECT DISTINCT
                                                    0 AS PEGWINDOW, TESTCASENAME, peg.FEEDPROCESSDETAILSID,
                                                    (
                                                      SELECT COUNT(1)
                                                      FROM IFORDER ifo
                                                      WHERE ifo.FEEDPROCESSDETAILSID = peg.FEEDPROCESSDETAILSID
                                                        AND ifo.TESTCASENAME = peg.TESTCASENAME
                                                        --AND ifo.PEGWINDOWSTEPID = peg.PEGWINDOW
                                                    ) as IFCOUNT,
                                                    (
                                                      SELECT COUNT(1)
                                                      FROM ELSEORDER ifo
                                                      WHERE ifo.FEEDPROCESSDETAILSID = peg.FEEDPROCESSDETAILSID
                                                        AND ifo.TESTCASENAME = peg.TESTCASENAME
                                                        --AND ifo.PEGWINDOWSTEPID = peg.PEGWINDOW
                                                    ) as ELSECOUNT,
                                                    (
                                                      SELECT COUNT(1)
                                                      FROM IFENDORDER ifo
                                                      WHERE ifo.FEEDPROCESSDETAILSID = peg.FEEDPROCESSDETAILSID
                                                        AND ifo.TESTCASENAME = peg.TESTCASENAME
                                                        --AND ifo.PEGWINDOWSTEPID = peg.PEGWINDOW
                                                    ) as IFENDCOUNT
                                                  FROM PEGWINODWORDER peg
                                                ) cte
                                                INNER JOIN tblstgtestcase t ON t.FEEDPROCESSDETAILID = cte.FEEDPROCESSDETAILSID
                                                  AND t.TESTCASENAME = cte.TESTCASENAME
                                                INNER JOIN tblfeedprocessdetails fdp ON fdp.FEEDPROCESSDETAILID = t.FEEDPROCESSDETAILID
                                                WHERE 
                                                (
                                                  (cte.IFCOUNT < cte.ELSECOUNT)
                                                  OR
                                                  (cte.IFCOUNT <> cte.IFENDCOUNT)
                                                  OR
                                                  (cte.ELSECOUNT > cte.IFENDCOUNT)
                                                );

                                              --==========================================

                                              EXECUTE IMMEDIATE 'DELETE FROM IFORDER';
                                              EXECUTE IMMEDIATE 'DELETE FROM ELSEORDER';
                                              EXECUTE IMMEDIATE 'DELETE FROM IFENDORDER';
                                              EXECUTE IMMEDIATE 'DELETE FROM PEGWINODWORDER';



                                            END;
                                            /*
                                                SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=25;
                                                SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=25;

                                                INSERT INTO "TBLLOGREPORT"
                                                    (
                                                        CREATIONDATE,
                                                        MESSAGETYPE,
                                                        ACTION,
                                                        CELLADDRESS,
                                                        VALIDATIONNAME,
                                                        VALIDATIONDESCRIPTION,
                                                        APPLICATIONNAME,
                                                        TESTCASENAME,
                                                        DATASETNAME,
                                                        TESTSTEPNUMBER,
                                                        OBJECTNAME,
                                                        COMMENTDATA,
                                                        ERRORDESCRIPTION,
                                                        PROGRAMLOCATION,
                                                        FEEDPROCESSINGDETAILSID,
                                                        FEEDPROCESSID
                                                    )
                                                    SELECT DISTINCT
                                                        (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                        VALIDATETYPE,
                                                        'Validation',
                                                        '',
                                                        VALIDATETYPE,
                                                        ERROR,
                                                        t.APPLICATION,
                                                        t.TESTCASENAME,
                                                        '',
                                                        t.rownumber,
                                                        t.object,
                                                        '',
                                                        ERROR,
                                                        '',
                                                        I_INDEX.FEEDPROCESSDETAILID,
                                                        FEEDPROCESSID1
                                            FROM tblstgtestcase t
                                            LEFT JOIN tblstgtestcase t1 ON t1.feedprocessdetailid = t.feedprocessdetailid
                                                AND t1.TESTSUITENAME = t.TESTSUITENAME
                                                AND t1.TESTCASENAME = t.TESTCASENAME
                                                AND t1.datasetname = t.datasetname
                                                AND t1.KEYWORD = 'IF'
                                            LEFT JOIN tblstgtestcase t2 ON t2.feedprocessdetailid = t.feedprocessdetailid
                                                AND t2.TESTSUITENAME = t.TESTSUITENAME
                                                AND t2.TESTCASENAME = t.TESTCASENAME
												AND t2.datasetname = t.datasetname
                                                AND LOWER(t2.KEYWORD) = 'pegwindow'
                                            where t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND t.KEYWORD IN ('ELSE', 'IFEND')
                                                AND (t.ID < t1.ID OR t1.ID IS NULL)
                                                AND t2.ID < t.ID
                                                AND (t2.ID < t1.ID OR t1.ID IS NULL);
                                        */
                                        -- IF ELSE  Order in Spreadsheet - Start    

                                        -- Check Object exist in the system or not Start
                                          SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=5;
                                          SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=5;

                                          INSERT INTO "TBLLOGREPORT"
                                          (
                                              CREATIONDATE,
                                              MESSAGETYPE,
                                              ACTION,
                                              CELLADDRESS,
                                              VALIDATIONNAME,
                                              VALIDATIONDESCRIPTION,
                                              APPLICATIONNAME,
                                              TESTCASENAME,
                                              DATASETNAME,
                                              TESTSTEPNUMBER,
                                              OBJECTNAME,
                                              COMMENTDATA,
                                              ERRORDESCRIPTION,
                                              PROGRAMLOCATION,
                                              FEEDPROCESSINGDETAILSID,
                                              FEEDPROCESSID
                                          )
                                          SELECT DISTINCT
                                            (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                            VALIDATETYPE,
                                            'Validation',
                                            '',
                                            VALIDATETYPE,
                                            ERROR,
                                            t.APPLICATION,
                                            t.TESTCASENAME,
                                            '',
                                            t.rownumber,
                                            t.object,
                                            '',
                                            ERROR,
                                            '',
                                            I_INDEX.FEEDPROCESSDETAILID,
                                            FEEDPROCESSID1
                                          from tblstgtestcase t
                                          where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                            AND t.OBJECT IS NOT NULL
                                            AND NOT EXISTS (
                                              SELECT 1
                                              FROM T_OBJECT_NAMEINFO
                                              WHERE T_OBJECT_NAMEINFO.OBJECT_HAPPY_NAME = t.OBJECT
                                            ); 
                                        -- Check Object exist in the system or not End

                                        END;    

                                        dbms_output.put_line('Log Insert Ended');

                                        -- Performance Code -- Shivam - End

                                        dbms_output.put_line('Log Insert Ended');

                                    -- Code added by Shivam - To save the errorlog for tab with wrong application - Start
                                        execute immediate 'TRUNCATE TABLE TEMP1';
                                        execute immediate 'TRUNCATE TABLE TEMPSTGTESTCASE';
                                        execute immediate 'TRUNCATE TABLE TEMPDWTESTCASE';

                                --END; --- LOGIC FOR NOT OVERWRITE - END
            ---------------------------------------------------------------------------------------------------------- LOGIC FOR NOT OVERWRITE - END ----------------------------------------------------------------------------------------------------------
                                --END IF; -- IF - 3 - END

            -------------- LOGIC FOR ANOTHER CURSOR WHERE DATASETMODE IS NOT NULL  --- START-----------------------------------

                                -- Performance change by Shivam Parikh - END
            -------------------------------------------------------------------------------
            --------------------LOGIC FOR MODE D - START ----------------------------------

                                -- Performance Code by Shivam Parikh - Start
                                DECLARE 
                                    ERROR varchar2(5000);
                                    MESSAGE VARCHAR2(500);
                                    MESSAGESYMBOL VARCHAR2(500);
                                    MaxRow INT;
                                BEGIN
                                    /*
                                    -- Insert log for not found Test Case - Start
                                    SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=14;
                                    SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=14;

                                    INSERT INTO "TBLLOGREPORT"
                                    (
                                        CREATIONDATE,
                                        MESSAGETYPE,
                                        ACTION,
                                        CELLADDRESS,
                                        VALIDATIONNAME,
                                        VALIDATIONDESCRIPTION,
                                        APPLICATIONNAME,
                                        TESTCASENAME,
                                        DATASETNAME,
                                        TESTSTEPNUMBER,
                                        OBJECTNAME,
                                        COMMENTDATA,
                                        ERRORDESCRIPTION,
                                        PROGRAMLOCATION,
                                        FEEDPROCESSINGDETAILSID,
                                        FEEDPROCESSID
                                    )
                                    SELECT 
                                        (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                        VALIDATETYPE,
                                        'Validation',
                                        '',
                                        'TESTCASE Validation',
                                        ERROR,
                                        APPLICATION,
                                        TESTCASENAME,
                                        DATASETNAME,
                                        '',
                                        '',
                                        '',
                                        ERROR,
                                        '',
                                        I_INDEX.FEEDPROCESSDETAILID,
                                        FEEDPROCESSID1
                                    FROM (    
                                        SELECT DISTINCT tblstgtestcase.TESTCASENAME,tblstgtestcase.DATASETNAME ,tblstgtestcase.APPLICATION
                                        FROM tblstgtestcase
                                        INNER JOIN T_TEST_
                                        WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                            AND TBLSTGTESTCASE.DATASETMODE='D'
                                             -- and tblstgtestcase.KEYWORD IS NOT NULL 
                                            AND NOT EXISTS (
                                                SELECT 1
                                                FROM T_TEST_CASE_SUMMARY
                                                WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(tblstgtestcase.TESTCASENAME)
                                            )
                                    ) xyz;
                                    -- Insert log for not found Test Case - End
                                    */

                                    -- Insert log for not found Data Summary - Start

                                    SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=15;
                                    SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=15;

                                    INSERT INTO "TBLLOGREPORT"
                                    (
                                        CREATIONDATE,
                                        MESSAGETYPE,
                                        ACTION,
                                        CELLADDRESS,
                                        VALIDATIONNAME,
                                        VALIDATIONDESCRIPTION,
                                        APPLICATIONNAME,
                                        TESTCASENAME,
                                        DATASETNAME,
                                        TESTSTEPNUMBER,
                                        OBJECTNAME,
                                        COMMENTDATA,
                                        ERRORDESCRIPTION,
                                        PROGRAMLOCATION,
                                        FEEDPROCESSINGDETAILSID,
                                        FEEDPROCESSID
                                    )
                                    SELECT (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,VALIDATETYPE,'Validation','','DATASUMMARY Validation',ERROR,APPLICATION,TESTCASENAME,DATASETNAME,'','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1
                                    FROM (    
                                        SELECT DISTINCT tblstgtestcase.TESTCASENAME,tblstgtestcase.DATASETNAME ,tblstgtestcase.APPLICATION
                                        FROM tblstgtestcase
                                        WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                            AND TBLSTGTESTCASE.DATASETMODE='D'
                                             and tblstgtestcase.KEYWORD IS NOT NULL 
                                            AND NOT EXISTS (
                                                SELECT 1
                                                FROM T_TEST_DATA_SUMMARY
                                                WHERE UPPER(t_test_data_summary.alias_name)=UPPER(tblstgtestcase.DATASETNAME)
                                                AND ROWNUM=1
                                            )
                                    ) xyz; 
                                    -- Insert log for not found Data Summary - End

                                    -- Insert log for not found Data Summary and Data Set both - Start

                                    SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=16;
                                    SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=16;

                                    INSERT INTO "TBLLOGREPORT"
                                    (
                                        CREATIONDATE,
                                        MESSAGETYPE,
                                        ACTION,
                                        CELLADDRESS,
                                        VALIDATIONNAME,
                                        VALIDATIONDESCRIPTION,
                                        APPLICATIONNAME,
                                        TESTCASENAME,
                                        DATASETNAME,
                                        TESTSTEPNUMBER,
                                        OBJECTNAME,
                                        COMMENTDATA,
                                        ERRORDESCRIPTION,
                                        PROGRAMLOCATION,
                                        FEEDPROCESSINGDETAILSID,
                                        FEEDPROCESSID
                                    )
                                    SELECT 
                                        (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                        VALIDATETYPE,
                                        'Validation',
                                        '',
                                        VALIDATETYPE,
                                        ERROR,
                                        APPLICATION,
                                        TESTCASENAME,
                                        DATASETNAME,
                                        '',
                                        '',
                                        '',
                                        ERROR,
                                        '',
                                        I_INDEX.FEEDPROCESSDETAILID,
                                        FEEDPROCESSID1
                                    FROM (    
                                        SELECT DISTINCT tblstgtestcase.TESTCASENAME,tblstgtestcase.DATASETNAME ,tblstgtestcase.APPLICATION
                                        FROM tblstgtestcase
                                        WHERE TBLSTGTESTCASE.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                            AND TBLSTGTESTCASE.DATASETMODE='D'
                                             and tblstgtestcase.KEYWORD IS NOT NULL 
                                            AND NOT EXISTS (
                                                SELECT 1
                                                FROM T_TEST_DATA_SUMMARY
                                                WHERE UPPER(t_test_data_summary.alias_name)=UPPER(tblstgtestcase.DATASETNAME)
                                                AND ROWNUM=1
                                            )
                                            AND NOT EXISTS (
                                                SELECT 1
                                                FROM T_TEST_CASE_SUMMARY
                                                WHERE UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(tblstgtestcase.TESTCASENAME) 
                                                AND ROWNUM=1
                                            )
                                    ) xyz; 
                                    -- Insert log for not found Data Summary and Data Set both - END

                                END;
                                -- Performance Code by Shivam Parikh - End

            -------------------------------------------------------------------------------
            -------------------LOGIC FOR MODE D - END -------------------------------------
            -------------- LOGIC FOR ANOTHER CURSOR WHERE DATASETMODE IS NOT NULL  --- END-----------------------------------


                                END;
                        ELSE
                                DECLARE
                                    VALIDTESTSUITENAME VARCHAR2(5000);
                                    ERRORTESTCASENAME VARCHAR2(5000);
                                     VALIDATEMESSAGE VARCHAR2(5000);
                                     VALIDATETYPE VARCHAR2(5000);
                                BEGIN
                                    SELECT MESSAGE INTO VALIDATEMESSAGE FROM TBLMESSAGE WHERE ID=11;
                                    SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=11;
                                    SELECT VALIDTESTSUITENAME INTO TESTSUITENAME FROM TBLSTGTESTCASE WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID AND ROWNUM=1;
                                    SELECT TESTCASENAME INTO ERRORTESTCASENAME FROM TBLSTGTESTCASE WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID AND ROWNUM=1;


                                    INSERT INTO "TBLLOGREPORT"(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,APPLICATIONNAME,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
                                    VALUES((SELECT SYSDATE FROM DUAL),VALIDATETYPE,'Validation','','TestSuite Name Validation',VALIDATEMESSAGE,VALIDTESTSUITENAME,ERRORTESTCASENAME,'','','','',VALIDATEMESSAGE,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1);

                                END;
                            END IF; -- IF - 2 - END

                END;
                                ELSE
									DECLARE
											ERRORAPPLICATIONNAME VARCHAR2(5000);
                                            ERRORTESTCASENAME VARCHAR2(5000);
                                            VALIDATEMESSAGE VARCHAR2(5000);
                                            VALIDATETYPE VARCHAR2(5000);
									BEGIN

											-- ERROR MESSAGE OF APPLICATION ID IS NOT FOUND IN DATABASE
											SELECT MESSAGE INTO VALIDATEMESSAGE FROM TBLMESSAGE WHERE ID=12;
											SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=12;
											SELECT APPLICATION INTO ERRORAPPLICATIONNAME FROM TBLSTGTESTCASE WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID AND ROWNUM=1;
                                            SELECT TESTCASENAME INTO ERRORTESTCASENAME FROM TBLSTGTESTCASE WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID AND ROWNUM=1;

											INSERT INTO "TBLLOGREPORT"(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,APPLICATIONNAME,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
											VALUES((SELECT SYSDATE FROM DUAL),VALIDATETYPE,'Validation','','Application Name Validation',VALIDATEMESSAGE,ERRORAPPLICATIONNAME,ERRORTESTCASENAME,'','','','',VALIDATEMESSAGE,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1);

									END;
                                END IF;
                          END;
                        END IF;
------------------------------------------------------- END - CODE FOR IMPORT TEST CASE ---------------------------------
--------------------------------------------------------------------------------------------------------------------------


----------------------------------------------------- START - CODE FOR IMPORT STORYBOARD ---------------------------------
-------------------------------------------------------------------------------------------------------------------------
  IF I_INDEX.FILETYPE = 'STORYBOARD' THEN
          DECLARE
            CURRENTDATE TIMESTAMP;
        --    ApplicationName VARCHAR2(5000);
        -- Remove duplicate staging rows
        BEGIN
        DELETE FROM tblstgstoryboard
           WHERE ID IN (
             SELECT DISTINCT ID
             from tblstgstoryboard t
             where t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
             AND EXISTS (
               SELECT 1
               FROM tblstgstoryboard s
               WHERE s.ID > t.ID
               AND s.feedprocessdetailid = t.feedprocessdetailid
               AND (               
              s.feedprocessdetailid = s.feedprocessdetailid and s.tabname = t.tabname and s.runorder = t.runorder and NVL(s.applicationname, 'N/A') = NVL(t.applicationname , 'N/A')
              and NVL(s.projectname, 'N/A') = NVL(t.projectname, 'N/A') and NVL(s.projectdetail, 'N/A')= NVL(t.projectdetail, 'N/A') and NVL(s.storyboardname, 'N/A')= NVL(t.storyboardname, 'N/A') and 
              s.actionname = t.actionname and NVL(s.stepname, 'N/A')= NVL(t.stepname, 'N/A') and NVL(s.suitename, 'N/A') = NVL( t.suitename, 'N/A') 
              and NVL(s.casename, 'N/A') = NVL( t.casename, 'N/A') and NVL(s.datasetname, 'N/A') = NVL(t.datasetname , 'N/A')
              and NVL(s.dependency, 'N/A') = NVL(t.dependency, 'N/A')
               )
            )
        );    
        commit;    

		SELECT SYSDATE INTO CURRENTDATE FROM DUAL;  


        insert into Storyboard_Temp (Name,Type,StoryboardName)        
        select distinct regexp_substr(applicationname,'[^,]+', 1, level), 'APPLICATION',StoryboardName from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
        connect by regexp_substr(applicationname, '[^,]+', 1, level) is not null; 

        insert into Storyboard_Temp(Name,Type,Description,StoryboardName)
        select distinct projectname,'PROJECT',projectdetail,StoryboardName from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

        insert into Storyboard_Temp(Name,Type,Description,StoryboardName)
        select distinct storyboardname,'STORYBOARD',projectname,StoryboardName from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

        insert into Storyboard_Temp(Name,Type,StoryboardName)
        select distinct casename,'TESTCASE',StoryboardName from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

        insert into Storyboard_Temp(Name,Type,StoryboardName)
        select distinct suitename,'TESTSUITE',StoryboardName from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

        insert into Storyboard_Temp(Name,Type,StoryboardName)
        select distinct StepName,'STEPNAME',StoryboardName from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID and StepName is not null;

        insert into Storyboard_Temp(Name,Type,StoryboardName)
        select distinct dependency,'DPENDENCY',StoryboardName from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID and dependency is not null;

        insert into Storyboard_Temp(Name,Type,StoryboardName)
        select distinct datasetname,'DATASET',StoryboardName from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

        insert into Storyboard_Temp(Name,Type,description)
        select distinct suitename,'RelTESTSUITEPROJECT',projectname from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID;

        --region find different application name in multiple sheets
        DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=51;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=51;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,general,APPLICATIONNAME,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Multiple sheets has different Application name',ERROR,'ApplicationName# ' || applicationname,applicationname,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from
            ( select applicationname from (
            select Rownum as ID,applicationname from (
            select applicationname from 
            tblstgstoryboard
            where FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID 
            group by applicationname )) where ID > 1  ) 
            group by CURRENTDATE,ERROR_TYPE,'Validation','','Multiple sheets has different Application name',ERROR,'ApplicationName# ' || applicationname,applicationname,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1;



         END;
         --endregion find different application name in multiple sheets

        --region find different project name and details in multiple sheets
        DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=52;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=52;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,PROJECTNAME,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Multiple sheets has different Project name and Details',ERROR,projectname,'ProjectName# ' ||projectname || ' ProjectDetail# ' || projectdetail,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from
            (  select projectname,projectdetail from (
            select Rownum as ID,projectname,projectdetail from (
            select projectname,projectdetail from 
            tblstgstoryboard
            where FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID 
            group by projectname,projectdetail )) where ID > 1  )
            group by CURRENTDATE,ERROR_TYPE,'Validation','','Multiple sheets has different Project name and Details',ERROR,projectname,'ProjectName# ' ||projectname || ' ProjectDetail# ' || projectdetail,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1
            ;

         END;
         --endregion find different project name and details in multiple sheets

        --region application
        DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=30;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=30;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,APPLICATIONNAME,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Application name has not exist in system',ERROR,sa.name,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from Storyboard_Temp sa
            left join t_registered_apps tr on tr.app_short_name = sa.name
            where tr.application_id is null and sa.name is not null and sa.Type = 'APPLICATION';

         END;
         --endregion application

        -- check into test case 
        DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=34;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=34;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','TestCase has not exist in system',ERROR,'',sa.StoryboardName ,'StoryboardName# '|| sa.StoryboardName || ' TestCaseName# ' || sa.name,sa.name,'','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from Storyboard_Temp sa
            left join t_test_case_summary tr on tr.test_case_name = sa.name
            where sa.name is not null and sa.Type = 'TESTCASE' and tr.test_case_id is null;

         END;
         -- endregion for test case

         -- check  test suite 
        DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=33;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=33;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,TESTSUITE,general,APPLICATIONNAME,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','TestSuite has not exist in system',ERROR,sa.StoryboardName,sa.name,'StoryboardName# '|| sa.StoryboardName || ' TESTSUITENAME#' || sa.name,'','','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from Storyboard_Temp sa
            left join t_test_suite tr on tr.test_suite_name = sa.name
            where sa.name is not null and sa.Type = 'TESTSUITE' and tr.test_suite_id is null;

         END;
         -- endregion for test suite

           -- check   dataset 
        DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=35;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=35;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,APPLICATIONNAME,TESTCASENAME,general,STORYBOARDNAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Dataset has not exist in system',ERROR,'','','StoryboardName# '|| sa.StoryboardName || ' DATASET# ' ||sa.name,sa.StoryboardName,sa.name,'','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from Storyboard_Temp sa
            left join t_test_data_summary tr on tr.alias_name = sa.name
            where sa.name is not null and sa.Type = 'DATASET' and tr.data_summary_id is null;

         END;
         -- endregion for  dataset

        --region duplicate step name not allow for Storyboard
         DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=40;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=40;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Duplicate Step not allow for Storyboard',ERROR,storyboardname,'StoryBoardName# '||storyboardname || ' ,StepName# ' || stepname,'','',stepname,'','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1             
            from ( select storyboardname,stepname from tblstgstoryboard where feedprocessdetailid= I_INDEX.FEEDPROCESSDETAILID
             and stepname is not null
            group by storyboardname,stepname having count(*) > 1
            )            
            group by CURRENTDATE,ERROR_TYPE,'Validation','','Duplicate Step not allow for Storyboard',ERROR,storyboardname,'StoryBoardName# '||storyboardname || ' ,StepName# ' || stepname,'','',stepname,'','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 
            ;

           -- select storyboardname,stepname,count(*) 

         END;
         --endregion





         --region check test case and test suite mapping
           DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=38;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=38;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,
            general,TESTCASENAME,TESTSUITE,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Relation between Testcase and Testsuite has not exist',ERROR,sa.storyboardname ,
            'StoryboardName# ' || sa.storyboardname ||' TestCaseName# '||sa.casename||' TestSuiteName# '||sa.suitename,sa.casename,sa.suitename,'','','',
            '',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from tblstgstoryboard sa            
            join t_test_suite tr on tr.test_suite_name = sa.suitename
            join t_test_case_summary tc on tc.test_case_name = sa.casename
            left join REL_TEST_CASE_TEST_SUITE rels on rels.test_case_id = tc.test_case_id and rels.test_suite_id = tr.test_suite_id
            where rels.relationship_id is null and sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID ;

         END;
         --endregion


         --region check test case and dataset mapping
           DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=39;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=39;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Relation between Testcase and Dataset has not exist',ERROR,sa.storyboardname,'StoryboardName# ' || sa.storyboardname ||' TestCaseName# '||sa.casename||' , Dataset# '||sa.datasetname,sa.casename,sa.datasetname,'','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from tblstgstoryboard sa            
            join t_test_data_summary tr on tr.alias_name = sa.datasetname
            join t_test_case_summary tc on tc.test_case_name = sa.casename
            left join REL_TC_DATA_SUMMARY rels on rels.test_case_id = tc.test_case_id and rels.data_summary_id = tr.data_summary_id
            where rels.id is null and sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID ;

         END;
         --endregion

         --region check test case and test suite mapping
      --  DECLARE
      --  ERROR VARCHAR2(500);
		--ERROR_TYPE VARCHAR2(500);

      --  BEGIN
         --   SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=41;
         --   SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=41;

          --  insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
          --  OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
          --  select CURRENTDATE,ERROR_TYPE,'Validation','','select * from TBLMESSAGE',ERROR,'StoryboardName# '||sa.storyboardname||' , Dataset#'||sa.datasetname,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from tblstgstoryboard sa            
           -- --join t_test_data_summary tr on upper(tr.alias_name) = upper(sa.datasetname)
          --  join t_test_suite trs on trs.test_suite_name = sa.suitename
           -- join t_test_case_summary tcs on tcs.test_case_name = sa.casename
          --  join t_test_project prj  on sa.projectname = prj.project_name
		--	left join REL_TEST_CASE_TEST_SUITE rel_tc_ts ON rel_tc_ts.test_case_id = tcs.test_case_id AND rel_tc_ts.test_suite_ID = trs.test_suite_id
           -- join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
           -- --join T_PROJ_TC_MGR mgr on tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id and trs.test_suite_id = mgr.test_suite_id 
                      --  --and tcs.test_case_id = mgr.test_case_id
           -- --left join T_STORYBOARD_DATASET_SETTING rels on rels.data_summary_id = tr.data_summary_id and rels.Storyboard_Detail_ID = mgr.storyboard_detail_id
            --where rel_tc_ts.relationship_id is null and sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID ;

         --END;
         --endregion


         --region dependency of step name 
         DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=36;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=36;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,DEPENDENCY,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Dependency not valid',ERROR, sb1.storyboardname,sb1.dependency,' StoryboardName# ' || sb1.storyboardname ||' StepName# '||sb1.stepname||' ,Dependency# '||sb1.dependency||' , '||sb1.storyboardname,'','',sb1.stepname,'','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from tblstgstoryboard sb1            
            left join tblstgstoryboard sb2 on sb1.storyboardname = sb2.storyboardname and sb1.projectname = sb2.projectname and sb1.dependency = sb2.stepname 
            where ((sb1.id <= sb2.id) or (sb2.Id is null))  and sb1.dependency is not null
            and sb1.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID and sb2.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
            order by 1;


            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,DEPENDENCY,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Dependency not valid',ERROR, sb1.storyboardname,sb1.dependency,' StoryboardName# ' || sb1.storyboardname ||' StepName# '||sb1.stepname||' ,Dependency# '||sb1.dependency||' , '||sb1.storyboardname,'','',sb1.stepname,'','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from tblstgstoryboard sb1 
            where sb1.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID 
            and sb1.dependency not in (select sb2.stepname from tblstgstoryboard sb2 where sb2.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
            and sb1.storyboardname = sb2.storyboardname and sb1.projectname = sb2.projectname and sb2.stepname is not null) and sb1.dependency <> 'None';



         END;


         --endregion


         --region Repeat storyboard name validation
         DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=82;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=82;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Same storyboard has repeat in different import sheets',ERROR,storyboardname,'StoryBoardName# '||storyboardname ,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 
            from 
            ( select storyboardname ,tabname            
            from tblstgstoryboard
            where  feedprocessdetailid= I_INDEX.FEEDPROCESSDETAILID
            group by storyboardname,tabname
            having count(*) > 1
            )            
            group by storyboardname ,I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 
            having count(*) > 1;


         END;
         --endregion


         --region testsuite and Project relationship
         DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=37;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=37;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,TESTSUITE,PROJECTNAME,APPLICATIONNAME,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','TestSuite and Project Mapping not exist',ERROR,sa.name,sa.description,'','','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from Storyboard_Temp sa
            join t_test_suite ts on ts.test_suite_name = sa.name
            join t_test_project prj on prj.project_name = sa.description
            left join REL_TEST_SUIT_PROJECT tr on tr.test_suite_id =  ts.test_suite_id and prj.project_id = tr.project_id
            where tr.relationship_id is null and sa.name is not null and sa.Type = 'RelTESTSUITEPROJECT';

         END;
         --endregion testsuite and Project relationship
        End;             
        End If;

        commit;


------------------------------------------------------- END - CODE FOR IMPORT STORYBOARD---------------------------------
--------------------------------------------------------------------------------------------------------------------------


------------------------------------------------------- Start - CODE FOR Variable---------------------------------
--------------------------------------------------------------------------------------------------------------------------



IF I_INDEX.FILETYPE = 'VARIABLE' THEN
          DECLARE
            CURRENTDATE TIMESTAMP;
        --    ApplicationName VARCHAR2(5000);
        -- Remove duplicate staging rows
        BEGIN
        DELETE FROM tblstgvariable
           WHERE ID IN (
             SELECT DISTINCT ID
             from tblstgvariable t
             where t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
             AND EXISTS (
               SELECT 1
               FROM tblstgvariable s
               WHERE s.ID > t.ID
               AND s.feedprocessdetailid = t.feedprocessdetailid
               AND (               
              s.feedprocessdetailid = s.feedprocessdetailid 
              and NVL(s.name, 'N/A') = NVL(t.name, 'N/A')
              and NVL(s.type, 'N/A') = NVL(t.type, 'N/A')
             -- and NVL(s.value, 'N/A') = NVL(t.value, 'N/A')
              and NVL(s.base_comp, 'N/A') = NVL(t.base_comp, 'N/A')
               )
            )
        );    
        commit;    



        --region Global: Name Type Duplicate
        DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=42;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=42;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Global Variable Name duplicate',ERROR,'Global Variable Name# '|| sa.Name ||' Type# '|| sa.Type ,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from tblstgvariable sa
            where sa.Type = 'GLOBAL_VAR' and sa.feedprocessdetailid= I_INDEX.FEEDPROCESSDETAILID
            group by  CURRENTDATE,ERROR_TYPE,'Validation','','Global Variable Name duplicate',ERROR,'Global Variable Name# '|| sa.Name ||' Type# '|| sa.Type ,ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1
            Having count(*) > 1
            ;

         END;


         DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=47;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=47;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Baseline/Compare has null value for Global Variable ',ERROR,'Global Variable Name# '|| sa.Name ||' Type# '|| sa.Type || ' BaseLine/Compare#' || sa.base_comp ,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from tblstgvariable sa
            where sa.Type = 'GLOBAL_VAR' and sa.base_comp is not null and sa.feedprocessdetailid= I_INDEX.FEEDPROCESSDETAILID
            ;

         END;
         --endregion Global Name Type Duplicate

        -- region Modal/Loop: Name Type Base/Comp Duplicate
         DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=43;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=43;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Modal/Loop Variable Name Duplicate',ERROR,'Modal/Loop Variable Name# '|| sa.Name ||' Type# '|| sa.Type || ' Base/Comp# ' || sa.base_comp ,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from tblstgvariable sa
            where sa.Type in ('MODAL_VAR','LOOP_VAR') and sa.feedprocessdetailid= I_INDEX.FEEDPROCESSDETAILID
            group by  CURRENTDATE,ERROR_TYPE,'Validation','','Modal/Loop Variable Name Duplicate',ERROR,'Modal/Loop Variable Name# '|| sa.Name ||' Type# '|| sa.Type || ' Base/Comp# ' || sa.base_comp ,ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1
            Having count(*) > 1
            ;

         END;
         -- endregion Modal/Loop: Name Type Base/Comp Duplicate

         --region Global -- Model/Loop: Name Duplicate
        DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=46;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=46;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select CURRENTDATE,ERROR_TYPE,'Validation','','Variable name has duplicate in import sheets',ERROR,'Variable Name# '|| sa.Name ,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from tblstgvariable sa
            join tblstgvariable sa1 on upper(sa1.Name) = upper(sa.Name)
            where sa.Type = 'GLOBAL_VAR' and sa1.Type in ('MODAL_VAR','LOOP_VAR') and sa.feedprocessdetailid= I_INDEX.FEEDPROCESSDETAILID and  sa1.feedprocessdetailid= I_INDEX.FEEDPROCESSDETAILID

            ;

         END;
         --endregion Global Name Type Duplicate


         -- Exist In Modal/Loop
        DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=44;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=44;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select distinct CURRENTDATE,ERROR_TYPE,'Validation','','Global variable name has exist in Modal/Loop Variable name',ERROR,'Global Variable Name# '|| sa.Name ||' Type# '|| sa.Type ,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from tblstgvariable sa
            join system_lookup lkp on Upper(lkp.Field_Name) = Upper(sa.Name)
            where sa.Type in ('GLOBAL_VAR') and lkp.table_name in ('MODAL_VAR','LOOP_VAR') and sa.feedprocessdetailid= I_INDEX.FEEDPROCESSDETAILID
            ;
         END;
         -- Exist In Modal/Loop

          -- Exist In Global
        DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);

        BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=45;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=45;

            insert into TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,general,TESTCASENAME,DATASETNAME,TESTSTEPNUMBER,
            OBJECTNAME,COMMENTDATA,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,FEEDPROCESSID)
            select distinct CURRENTDATE,ERROR_TYPE,'Validation','','Modal/Loop variable name has exist in Global Variable name',ERROR,'Modal/Loop Variable Name# '|| sa.Name ||' Type# '|| sa.Type || ' Base/Comp#' || sa.base_comp,'','','','','',ERROR,'',I_INDEX.FEEDPROCESSDETAILID,FEEDPROCESSID1 from tblstgvariable sa
            join system_lookup lkp on Upper(lkp.Field_Name) = Upper(sa.Name)
            where lkp.table_name in ('GLOBAL_VAR') and sa.Type in ('MODAL_VAR','LOOP_VAR') and sa.feedprocessdetailid= I_INDEX.FEEDPROCESSDETAILID 
            ;
         END;
         -- Exist In Global

         --endregion
        End;             
        End If;



------------------------------------------------------- End - CODE FOR Variable---------------------------------
--------------------------------------------------------------------------------------------------------------------------


			END LOOP;


-----------------------------------START-CODE FOR CHECK TBLLOGREPORT TABLE COUNT------------------------------------------
--------------------------------------------------------------------------------------------------------------------------

SELECT COUNT(*) INTO CHECKRESULT
    from tblLOGREPORT 
    INNER JOIN tblfeedprocessdetails ON tblfeedprocessdetails.feedprocessid = tbllogreport.feedprocessid
    where 
         tblfeedprocessdetails.feedprocessid=  FEEDPROCESSID1;

IF CHECKRESULT != 0 THEN
    RESULT:= 'ERROR';
END IF; 


-----------------------------------START-CODE FOR CHECK TBLLOGREPORT TABLE COUNT------------------------------------------
--------------------------------------------------------------------------------------------------------------------------







END;
END USP_MAPPING_VALIDATION;
/
create or replace PROCEDURE USP_VALIDATION(
DATAVALUE IN VARCHAR2,
VALIDATETYPE IN VARCHAR2,
ID OUT NUMBER)
IS
BEGIN
	DECLARE
		COUNTTESTSUITE  NUMBER:=0;
	BEGIN
		IF VALIDATETYPE='SUITENAME' THEN
			BEGIN
				SELECT COUNT(*) INTO COUNTTESTSUITE FROM T_TEST_SUITE WHERE UPPER(T_TEST_SUITE.TEST_SUITE_NAME)=UPPER(DATAVALUE);

			END;
        ELSIF VALIDATETYPE='PROJECTNAME' THEN
			BEGIN
				SELECT COUNT(*) INTO COUNTTESTSUITE FROM t_test_project WHERE UPPER(project_name)=UPPER(DATAVALUE);

			END;
		ELSE
			BEGIN
				SELECT COUNT(*) INTO COUNTTESTSUITE FROM T_REGISTERED_APPS WHERE UPPER(T_REGISTERED_APPS.APP_SHORT_NAME)=UPPER(DATAVALUE);

			END;
		END IF;


		--------------------
		IF COUNTTESTSUITE != 0 THEN
			BEGIN
				ID:=COUNTTESTSUITE;
			END;
		ELSE
			BEGIN
				ID:=0;
			END;
		END IF;

	END;

END USP_VALIDATION;
/

create or replace FUNCTION concatenate_application(test_suite_id1 NUMERIC)
  RETURN CLOB
IS
  l_return CLOB; 
BEGIN
  SELECT LISTAGG(APP_SHORT_NAME, ',') WITHIN GROUP (ORDER BY APP_SHORT_NAME) AS APP_SHORT_NAME INTO l_return
  FROM (
    SELECT DISTINCT APP_SHORT_NAME
    FROM rel_app_testsuite relapp 
    INNER JOIN t_registered_apps trap ON trap.APPLICATION_ID = relapp.APPLICATION_ID
    WHERE relapp.TEST_SUITE_ID = test_suite_id1
  ) xyz;  

  RETURN l_return;

END;
/

create or replace FUNCTION concatenate_datasetdescription(test_case_id1 NUMERIC)
  RETURN CLOB
IS
  l_return  CLOB; 
BEGIN
    SELECT RTRIM(XMLAGG(XMLELEMENT(E,x.DESCRIPTION_INFO,',').EXTRACT('//text()') ORDER BY x.DESCRIPTION_INFO).GetClobVal(),',') INTO l_return
  --SELECT LISTAGG(x.DESCRIPTION_INFO, ',') WITHIN GROUP (ORDER BY ALIAS_NAME) INTO l_return
  FROM (
        SELECT DISTINCT ttds.ALIAS_NAME,NVL(REPLACE(ttds.DESCRIPTION_INFO, ',', '~'), ' ') AS DESCRIPTION_INFO
        FROM REL_TC_DATA_SUMMARY reltcds 
        INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
        WHERE reltcds.TEST_CASE_ID = test_case_id1
        ORDER BY ttds.ALIAS_NAME
      ) x;
  RETURN l_return;

END;

/

create or replace FUNCTION concatenate_datasetids(test_case_id1 NUMERIC)
  RETURN CLOB
IS
  l_return CLOB; 
BEGIN
  SELECT RTRIM(XMLAGG(XMLELEMENT(E,reltcds.DATA_SUMMARY_ID,',').EXTRACT('//text()') ORDER BY ALIAS_NAME).GetClobVal(),',') INTO l_return
  FROM REL_TC_DATA_SUMMARY reltcds 
  INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
  WHERE reltcds.TEST_CASE_ID = test_case_id1;

  RETURN l_return;

END;
/

create or replace FUNCTION concatenate_datasetnames(test_case_id1 NUMERIC)
  RETURN CLOB
IS
  l_return CLOB; 
BEGIN
  SELECT RTRIM(XMLAGG(XMLELEMENT(E,ALIAS_NAME,',').EXTRACT('//text()') ORDER BY ALIAS_NAME).GetClobVal(),',') INTO l_return
  FROM REL_TC_DATA_SUMMARY reltcds 
  INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
  WHERE reltcds.TEST_CASE_ID = test_case_id1;

  RETURN l_return;

END;

/

create or replace FUNCTION concatenate_list (p_cursor IN  SYS_REFCURSOR)
  RETURN  VARCHAR2
IS
  l_return  VARCHAR2(32767); 
  l_temp    VARCHAR2(32767);
BEGIN
  LOOP
    FETCH p_cursor
    INTO  l_temp;
    EXIT WHEN p_cursor%NOTFOUND;
    l_return := l_return || ',' || NVL(l_temp, '');
  END LOOP;
  RETURN LTRIM(l_return, ',');
END;
/

create or replace FUNCTION concatenate_teststepskip(test_case_id1 NUMERIC, steps_id1 NUMERIC)
  RETURN CLOB
IS
  l_return  CLOB; 
BEGIN
  SELECT LISTAGG(x.DATA_DIRECTION, ',') WITHIN GROUP (ORDER BY ALIAS_NAME) INTO l_return
  FROM (
        SELECT DISTINCT ttds.ALIAS_NAME,NVL(testds.DATA_DIRECTION, 0) AS DATA_DIRECTION
        FROM REL_TC_DATA_SUMMARY reltcds
        INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
        INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID
        LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
          AND testds.steps_id = ts1.STEPS_ID
        WHERE reltcds.TEST_CASE_ID = test_case_id1
          AND ts1.STEPS_ID = steps_id1
        ORDER BY ttds.ALIAS_NAME 
      ) x;
  RETURN l_return;

END;
/

create or replace FUNCTION concatenate_teststepvalue(test_case_id1 NUMERIC, steps_id1 NUMERIC)
  RETURN CLOB
IS
  l_return CLOB; 
BEGIN
    SELECT RTRIM(XMLAGG(XMLELEMENT(E,DATA_VALUE,',').EXTRACT('//text()') ORDER BY ALIAS_NAME).GetClobVal(),',') INTO l_return
      FROM (
        SELECT DISTINCT ttds.ALIAS_NAME,
          (
            SELECT DISTINCT REPLACE(testds.DATA_VALUE, ',', '~') AS ALIAS_NAME
            FROM TEST_DATA_SETTING testds
            WHERE testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
              AND testds.steps_id = ts1.STEPS_ID
              AND ROWNUM = 1
              AND testds.DATA_SETTING_ID = (
                SELECT MAX(testds1.DATA_SETTING_ID)
                FROM TEST_DATA_SETTING testds1
                WHERE testds1.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
                    AND testds1.steps_id = testds.steps_id
              )
            --ORDER BY DATA_SETTING_ID DESC
          ) AS DATA_VALUE
        --NVL(REPLACE(testds.DATA_VALUE, ',', '~'), ' ') AS DATA_VALUE
        FROM REL_TC_DATA_SUMMARY reltcds
        INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
        INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID
          AND ts1.STEPS_ID = steps_id1
        --LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
          --AND testds.steps_id = ts1.STEPS_ID
        WHERE reltcds.TEST_CASE_ID = test_case_id1
          AND ts1.STEPS_ID = steps_id1
        ORDER BY ttds.ALIAS_NAME
     ) xyz;
  RETURN l_return;

END;

/

create or replace FUNCTION "GETTOPOBJECT" (OBJECTNAME IN VARCHAR2, APPLICATIONID INT)
  RETURN T_REGISTED_OBJECT_TOP_coll pipelined is
BEGIN
    IF OBJECTNAME IS NOT NULL THEN
    BEGIN
      FOR i in (
                select T_REGISTED_OBJECT.OBJECT_ID, 
                T_OBJECT_NAMEINFO.OBJECT_NAME_ID 
                from T_OBJECT_NAMEINFO 
                INNER JOIN T_REGISTED_OBJECT ON T_REGISTED_OBJECT.OBJECT_NAME_ID = T_OBJECT_NAMEINFO.OBJECT_NAME_ID 
                  AND (APPLICATIONID = 0 OR T_REGISTED_OBJECT.APPLICATION_ID = APPLICATIONID)
                where T_OBJECT_NAMEINFO.OBJECT_HAPPY_NAME = OBJECTNAME AND ROWNUM = 1) loop
        pipe row(T_REGISTED_OBJECT_TOP(i.OBJECT_ID,i.OBJECT_NAME_ID));
      end loop;
    END;
    ELSE
    BEGIN
        pipe row(T_REGISTED_OBJECT_TOP(NULL,NULL));
    END;
    END IF;
END;
/

