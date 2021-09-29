


create or replace procedure SP_GET_USER_CONFIGURATION(
sl_cursor OUT SYS_REFCURSOR
)
IS 
stm VARCHAR2(30000);

BEGIN

stm := 'select tpr.USERCONFIGID, nvl(tpr.MAINKEY, '''') as MAINKEY ,nvl(tpr.SUBKEY, '''') as SUBKEY, tpr.USERID,
nvl(tp.TESTER_LOGIN_NAME, '''') as MARSUserName,
--utl_raw.cast_to_varchar2(dbms_lob.substr(tpr.BLOBVALUE)) 
BLOBVALUE as BLOBValuestr,
tpr.BLOBVALUETYPE,
CASE 
        WHEN tpr.BLOBVALUETYPE = 1 then ''Json''
        ELSE ''Xml''
        END 
    AS BLOBType,
    nvl(tpr.DESCRIPTION, '''') as DESCRIPTION
from T_USER_CONFIGURATION tpr 
left join T_TESTER_INFO tp on tpr.USERID = tp.TESTER_ID';

OPEN sl_cursor FOR  stm ;
END SP_GET_USER_CONFIGURATION;

/
create or replace PROCEDURE             "SP_SaveStoryboard" (
    FEEDPROCESS_ID IN NUMBER,
   STORYBOARDID IN NUMBER,
   PROJECTID IN NUMBER,
  RESULT OUT CLOB
)
IS
BEGIN


   

MERGE INTO T_PROJ_TC_MGR mgr
  USING (  
select distinct stg.STORYBOARDDETAILID,ts.TEST_SUITE_ID,tc.TEST_CASE_ID,ds.DATA_SUMMARY_ID,sl.VALUE as Action,stg.STEPNAME
      ,stg.ROW_ID + 1 as ROW_ID
      from TBLSTGSTORYBOARDVALID stg
join SYSTEM_LOOKUP sl on sl.TABLE_NAME = 'T_PROJ_TC_MGR' and sl.FIELD_NAME = 'RUN_TYPE' 
    and lower(sl.DISPLAY_NAME) = lower(stg.ACTIONAME)
join T_TEST_SUITE ts on lower(ts.TEST_SUITE_NAME) = lower(stg.TESTSUITENAME)    
join T_TEST_CASE_SUMMARY tc on lower(tc.TEST_CASE_NAME) = lower(stg.TESTCASENAME)
join T_TEST_DATA_SUMMARY ds on lower(ds.ALIAS_NAME) = lower(stg.DATASETNAME)
where stg.FEEDPROCESSID = FEEDPROCESS_ID and stg.STORYBOARDDETAILID > 0 ) TMP
   ON (mgr.STORYBOARD_DETAIL_ID = TMP.STORYBOARDDETAILID)
   WHEN MATCHED THEN
    UPDATE SET
   mgr.TEST_SUITE_ID = TMP.TEST_SUITE_ID,
   mgr.TEST_CASE_ID = TMP.TEST_CASE_ID,
   mgr.RUN_TYPE = TMP.Action,
   mgr.RUN_ORDER = TMP.ROW_ID,
   mgr.ALIAS_NAME = TMP.STEPNAME
   ;
   commit;
   
   
MERGE INTO T_STORYBOARD_DATASET_SETTING sd
  USING (  
select distinct stg.STORYBOARDDETAILID,ds.DATA_SUMMARY_ID     
      from TBLSTGSTORYBOARDVALID stg
join T_TEST_DATA_SUMMARY ds on lower(ds.ALIAS_NAME) = lower(stg.DATASETNAME)
where stg.FEEDPROCESSID = FEEDPROCESS_ID and stg.STORYBOARDDETAILID > 0 ) TMP
   ON (sd.STORYBOARD_DETAIL_ID = TMP.STORYBOARDDETAILID)
   WHEN MATCHED THEN
    UPDATE SET
   sd.DATA_SUMMARY_ID = TMP.DATA_SUMMARY_ID;
 commit;
   
   
 
  insert into T_PROJ_TC_MGR(STORYBOARD_DETAIL_ID,TEST_SUITE_ID,TEST_CASE_ID,RUN_TYPE,ALIAS_NAME,RUN_ORDER,PROJECT_ID,STORYBOARD_ID)
select  T_TEST_STEPS_SEQ.nextval,ts.TEST_SUITE_ID,tc.TEST_CASE_ID,sl.VALUE as Action,stg.STEPNAME
      ,stg.ROW_ID + 1 as ROW_ID,PROJECTID,stg.STORYBOARDID
      from TBLSTGSTORYBOARDVALID stg
join SYSTEM_LOOKUP sl on sl.TABLE_NAME = 'T_PROJ_TC_MGR' and sl.FIELD_NAME = 'RUN_TYPE' 
    and lower(sl.DISPLAY_NAME) = lower(stg.ACTIONAME)
join T_TEST_SUITE ts on lower(ts.TEST_SUITE_NAME) = lower(stg.TESTSUITENAME)    
join T_TEST_CASE_SUMMARY tc on lower(tc.TEST_CASE_NAME) = lower(stg.TESTCASENAME)
join T_TEST_DATA_SUMMARY ds on lower(ds.ALIAS_NAME) = lower(stg.DATASETNAME)
where stg.FEEDPROCESSID = FEEDPROCESS_ID and stg.STORYBOARDDETAILID is null ;
 commit;
 
 
insert into T_STORYBOARD_DATASET_SETTING(SETTING_ID,STORYBOARD_DETAIL_ID,DATA_SUMMARY_ID)
select T_TEST_STEPS_SEQ.nextval,mgr.STORYBOARD_DETAIL_ID,ds.DATA_SUMMARY_ID from TBLSTGSTORYBOARDVALID stg
join T_TEST_DATA_SUMMARY ds on lower(ds.ALIAS_NAME) = lower(stg.DATASETNAME)
join T_PROJ_TC_MGR mgr on mgr.RUN_ORDER = (stg.ROW_ID + 1) and mgr.STORYBOARD_ID = stg.STORYBOARDID
where stg.FEEDPROCESSID = FEEDPROCESS_ID and stg.STORYBOARDDETAILID is null;
 commit;
 
 
--update Dependon
MERGE INTO T_PROJ_TC_MGR tmgr
  USING (  
select  mgr.STORYBOARD_DETAIL_ID as StID,dep.STORYBOARD_DETAIL_ID as UpdateValue
      from TBLSTGSTORYBOARDVALID stg
join T_PROJ_TC_MGR mgr on mgr.RUN_ORDER = (stg.ROW_ID + 1) and mgr.STORYBOARD_ID = stg.STORYBOARDID
join T_PROJ_TC_MGR dep on  mgr.STORYBOARD_ID = dep.STORYBOARD_ID and lower(dep.ALIAS_NAME) = lower(stg.DEPENDENCY)
where stg.FEEDPROCESSID = FEEDPROCESS_ID and stg.DEPENDENCY not in ('None') ) TMP
   ON (tmgr.STORYBOARD_DETAIL_ID = TMP.StID)
   WHEN MATCHED THEN
    UPDATE SET
   tmgr.DEPENDS_ON = TMP.UpdateValue
   ; commit;
   
   update T_TEST_DATA_SUMMARY set status = 0 where status is null;
   commit;


end "SP_SaveStoryboard";

/
create or replace PROCEDURE             "SP_CheckValidationStoryboard" (
    FEEDPROCESSID1 IN NUMBER,
   
  RESULT OUT CLOB
)
IS
BEGIN

insert into TBLSTGSTORYBOARDVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
select stg.ROW_ID,'Invalid Action','0',stg.FEEDPROCESSID from TBLSTGSTORYBOARDVALID stg where lower(stg.ACTIONAME) not in ('run','skip','done','execute')
and stg.FEEDPROCESSID = FEEDPROCESSID1 ;

insert into TBLSTGSTORYBOARDVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
select stg.ROW_ID,'Test Suite ['|| stg.TESTSUITENAME ||'] does not exist.','0',stg.FEEDPROCESSID from TBLSTGSTORYBOARDVALID stg
left join T_TEST_SUITE ts on lower(ts.TEST_SUITE_NAME) = lower(stg.TESTSUITENAME)
where stg.FEEDPROCESSID = FEEDPROCESSID1 and ts.TEST_SUITE_ID is null;


insert into TBLSTGSTORYBOARDVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
select stg.ROW_ID,'Test Case ['|| stg.TESTCASENAME ||'] does not exist.','0',stg.FEEDPROCESSID from TBLSTGSTORYBOARDVALID stg
left join T_TEST_CASE_SUMMARY tc on lower(tc.TEST_CASE_NAME) = lower(stg.TESTCASENAME)
where stg.FEEDPROCESSID = FEEDPROCESSID1 and tc.TEST_CASE_ID is null;


insert into TBLSTGSTORYBOARDVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
select stg.ROW_ID,'Dataset ['|| stg.DATASETNAME ||'] does not exist.','0',stg.FEEDPROCESSID from TBLSTGSTORYBOARDVALID stg
left join T_TEST_DATA_SUMMARY ds on lower(ds.ALIAS_NAME) = lower(stg.DATASETNAME)
where stg.FEEDPROCESSID = FEEDPROCESSID1 and ds.DATA_SUMMARY_ID is null;


insert into TBLSTGSTORYBOARDVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
select stg.ROW_ID,'Test Suite and Storyboard should be in same Project.','0',stg.FEEDPROCESSID  from TBLSTGSTORYBOARDVALID stg 
join T_TEST_SUITE ts on lower(ts.TEST_SUITE_NAME) = lower(stg.TESTSUITENAME)
join T_TEST_PROJECT prj on lower(prj.PROJECT_NAME) = lower(ts.TEST_SUITE_NAME)
join REL_TEST_SUIT_PROJECT relt on relt.PROJECT_ID = prj.PROJECT_ID and relt.TEST_SUITE_ID = ts.TEST_SUITE_ID
left join T_PROJ_TC_MGR mgr on mgr.PROJECT_ID = relt.PROJECT_ID and ts.TEST_SUITE_ID = mgr.TEST_SUITE_ID
where stg.FEEDPROCESSID = FEEDPROCESSID1 and mgr.STORYBOARD_DETAIL_ID is null ;

insert into TBLSTGSTORYBOARDVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
select stg.ROW_ID,'Test Suite ['|| stg.TESTSUITENAME ||'] does not contain Test Case ['|| stg.TESTCASENAME ||'].','0',stg.FEEDPROCESSID from TBLSTGSTORYBOARDVALID stg
join T_TEST_SUITE ts on lower(ts.TEST_SUITE_NAME) = lower(stg.TESTSUITENAME)
join T_TEST_CASE_SUMMARY tc on lower(tc.TEST_CASE_NAME) = lower(stg.TESTCASENAME)
left join REL_TEST_CASE_TEST_SUITE reltstc on reltstc.TEST_CASE_ID = tc.TEST_CASE_ID and reltstc.TEST_SUITE_ID = ts.TEST_SUITE_ID
where stg.FEEDPROCESSID = FEEDPROCESSID1 and reltstc.RELATIONSHIP_ID is null;


insert into TBLSTGSTORYBOARDVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
select stg.ROW_ID,'Test Case ['|| stg.TESTCASENAME ||'] does not contain Dataset ['|| stg.DATASETNAME ||'].','0',stg.FEEDPROCESSID from TBLSTGSTORYBOARDVALID stg
join T_TEST_CASE_SUMMARY tc on lower(tc.TEST_CASE_NAME) = lower(stg.TESTCASENAME)
join T_TEST_DATA_SUMMARY ds on lower(ds.ALIAS_NAME) = lower(stg.DATASETNAME)
left join REL_TC_DATA_SUMMARY tcds on tcds.DATA_SUMMARY_ID = ds.DATA_SUMMARY_ID and tcds.TEST_CASE_ID = tc.TEST_CASE_ID
where stg.FEEDPROCESSID = FEEDPROCESSID1 and tcds.ID is null;

  

end "SP_CheckValidationStoryboard";
/

create or replace PROCEDURE SP_GetSBValidationResult( 
FEEDPROCESSID1 IN NUMBER,
sl_cursor OUT SYS_REFCURSOR
)
IS
stm VARCHAR2(30000);
BEGIN
stm := '';
    stm := ' select distinct  ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID,FEEDPROCESSDETAILID from TBLSTGSTORYBOARDVALIDATION where
    FEEDPROCESSID = '||  FEEDPROCESSID1 ||'';
      OPEN sl_cursor FOR  stm ; 


END;

/

create or replace PROCEDURE          "SP_EXPORT_EXPORTDATATAGREPORT" (
sl_cursor OUT SYS_REFCURSOR
)
IS
	stm VARCHAR2(30000);

BEGIN

    stm := ' select distinct * from (select  tp.project_name,tss.storyboard_name, tpj.run_order, tsc.test_case_name, ts.alias_name,TS.DESCRIPTION_INFO, tg.groupname, tg.description as GROUPDESCRIPTION,  
           tts.setname, tts.description as SETDESCRIPTION, tf.foldername, tf.description as FOLDERDESCRIPTION,dts.EXPECTEDRESULTS, dts.sequence,dts.stepdesc, dts.diary from T_TEST_DATASETTAG dts 
           join t_test_data_summary ts ON dts.datasetId = ts.data_summary_id
           left join t_test_group tg on tg.groupid = dts.groupid
           left join T_test_set tts on tts.setid = dts.setid
           left join t_test_folder tf on tf.folderid = dts.folderid
           join REL_TC_DATA_SUMMARY rts on rts.data_summary_id = ts.data_summary_id
           join t_test_case_summary tsc on tsc.test_case_id = rts.test_case_id
           join t_proj_tc_mgr tpj on tpj.test_case_id = tsc.test_case_id
           join t_storyboard_dataset_setting tdss on tdss.data_summary_id = dts.datasetId and tpj.storyboard_detail_id=tdss.storyboard_detail_id
           join t_storyboard_summary tss on tss.storyboard_id = tpj.storyboard_id
           join T_TEST_PROJECT tp on tp.project_id = tpj.project_id
           order by  tg.groupname,tpj.run_order)';
    OPEN sl_cursor FOR  stm;
    END SP_EXPORT_EXPORTDATATAGREPORT;
	
	
	/
	
	create or replace PROCEDURE SP_SAVE_IMPORT_DATATAG
(
feeddetailid in varchar2
)AS 
BEGIN
DECLARE
    CURRENTDATE TIMESTAMP;
     BEGIN
       SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

        update t_test_group tg SET (tg.DESCRIPTION, tg.active,tg.UPDATE_DATE)=(
        select tbj.DESCRIPTION, tbj.active, CURRENTDATE from TBLSTGCOMMONDATASETTAG tbj
        where tbj.FEEDPROCESSDETAILID=feeddetailid and tbj.type='Group' and upper(tg.groupname) = upper(tbj.name)
        ) where EXISTS (
        select tbj.DESCRIPTION, tbj.active, CURRENTDATE from TBLSTGCOMMONDATASETTAG tbj
        where tbj.FEEDPROCESSDETAILID=feeddetailid and tbj.type='Group' and upper(tg.groupname) = upper(tbj.name));

        insert into T_TEST_GROUP (GROUPID, GROUPNAME, DESCRIPTION, ACTIVE, CREATION_DATE, UPDATE_DATE)
        select SEQ_T_TEST_GROUP.nextval,tbj.name, tbj.DESCRIPTION,tbj.active, CURRENTDATE , CURRENTDATE from TBLSTGCOMMONDATASETTAG tbj
        left join t_test_group tg on upper(tg.groupname) = upper(tbj.name)
        where FEEDPROCESSDETAILID=feeddetailid and tbj.type='Group' and tg.groupid is null;

       update T_test_set ts SET (ts.DESCRIPTION, ts.active ,ts.UPDATE_DATE)=(
       select tbj.DESCRIPTION,tbj.active, CURRENTDATE from TBLSTGCOMMONDATASETTAG tbj
       where tbj.FEEDPROCESSDETAILID=feeddetailid and tbj.type='Set' and upper(ts.setname) = upper(tbj.name)
       ) where EXISTS (
       select tbj.DESCRIPTION,tbj.active, CURRENTDATE from TBLSTGCOMMONDATASETTAG tbj
       where tbj.FEEDPROCESSDETAILID=feeddetailid and tbj.type='Set' and upper(ts.setname) = upper(tbj.name));

      insert into T_TEST_SET (SETID, SETNAME, DESCRIPTION, ACTIVE, CREATION_DATE, UPDATE_DATE)
      select SEQ_T_TEST_SET.nextval ,tbj.name, tbj.DESCRIPTION,tbj.active, CURRENTDATE, CURRENTDATE from TBLSTGCOMMONDATASETTAG tbj
      left join T_test_set ts on upper(ts.setname) = upper(tbj.name)
      where FEEDPROCESSDETAILID=feeddetailid and tbj.type='Set' and ts.setid is null;

      update t_test_folder tf SET (tf.DESCRIPTION, tf.active,tf.UPDATE_DATE)=(
      select tbj.DESCRIPTION,tbj.active, CURRENTDATE from TBLSTGCOMMONDATASETTAG tbj
      where tbj.FEEDPROCESSDETAILID=feeddetailid and tbj.type='Folder' and upper(tf.foldername) = upper(tbj.name)
      ) where EXISTS (
      select tbj.DESCRIPTION,tbj.active, CURRENTDATE from TBLSTGCOMMONDATASETTAG tbj
      where tbj.FEEDPROCESSDETAILID=feeddetailid and tbj.type='Folder' and upper(tf.foldername) = upper(tbj.name));

      insert into T_TEST_FOLDER (FOLDERID, FOLDERNAME, DESCRIPTION, ACTIVE, CREATION_DATE, UPDATE_DATE)
      select SEQ_T_TEST_FOLDER.nextval ,tbj.name, tbj.DESCRIPTION,tbj.active, CURRENTDATE, CURRENTDATE from TBLSTGCOMMONDATASETTAG tbj
      left join t_test_folder tf on upper(tf.foldername) = upper(tbj.name)
      where FEEDPROCESSDETAILID=feeddetailid and tbj.type='Folder' and tf.folderid is null;

     update T_TEST_DATASETTAG tdt SET (tdt.GROUPID, tdt.SETID, tdt.FOLDERID, tdt.EXPECTEDRESULTS,tdt.STEPDESC,tdt.DIARY,tdt.SEQUENCE)=(
     select tg.groupid,tts.setid, tf.folderid, tbj.EXPECTEDRESULTS,tbj.STEPDESC,tbj.DIARY,tbj.SEQUENCE from TBLSTGDATASETTAG tbj
     join T_Test_data_summary ts on upper(ts.alias_name) = upper(tbj.DATASETNAME) 
     left join t_test_group tg on upper(tg.groupname) = upper(tbj.TAGGROUP)
     left join T_test_set tts  on upper(tts.setname) = upper(tbj.TAGSET)
     left join t_test_folder tf on upper(tf.foldername) = upper(tbj.TAGFOLDER)
     where tbj.FEEDPROCESSDETAILID=feeddetailid  and tdt.DATASETID = ts.data_summary_id
     ) where EXISTS (
      select tg.groupid,tts.setid, tf.folderid, tbj.EXPECTEDRESULTS,tbj.STEPDESC,tbj.DIARY, tbj.SEQUENCE from TBLSTGDATASETTAG tbj
     join T_Test_data_summary ts on upper(ts.alias_name) = upper(tbj.DATASETNAME) 
     left join t_test_group tg on upper(tg.groupname) = upper(tbj.TAGGROUP)
     left join T_test_set tts  on upper(tts.setname) = upper(tbj.TAGSET)
     left join t_test_folder tf on upper(tf.foldername) = upper(tbj.TAGFOLDER)
     where tbj.FEEDPROCESSDETAILID=feeddetailid  and tdt.DATASETID = ts.data_summary_id);

     insert into T_TEST_DATASETTAG (T_TEST_DATASETTAG_ID, DATASETID,GROUPID, SETID, FOLDERID, EXPECTEDRESULTS, STEPDESC, DIARY, SEQUENCE)
     select SEQ_T_TEST_DATASETTAG.nextval,ts.data_summary_id,tg.groupid,tts.setid, tf.folderid, tbj.EXPECTEDRESULTS,tbj.STEPDESC,tbj.DIARY,tbj.SEQUENCE from TBLSTGDATASETTAG tbj
     join T_Test_data_summary ts on upper(ts.alias_name) = upper(tbj.DATASETNAME) 
     left join T_TEST_DATASETTAG tdt on tdt.DATASETID = ts.data_summary_id
     left join t_test_group tg on upper(tg.groupname) = upper(tbj.TAGGROUP)
     left join T_test_set tts  on upper(tts.setname) = upper(tbj.TAGSET)
     left join t_test_folder tf on upper(tf.foldername) = upper(tbj.TAGFOLDER)
     where tbj.FEEDPROCESSDETAILID=feeddetailid and tdt.DATASETID is null;
   
     update T_Test_data_summary ts SET (TS.DESCRIPTION_INFO)=(
     select TBJ.DESCRIPTION from TBLSTGDATASETTAG tbj
     where tbj.FEEDPROCESSDETAILID=feeddetailid and upper(ts.alias_name) = upper(tbj.DATASETNAME) ) where EXISTS (
     select TBJ.DESCRIPTION from TBLSTGDATASETTAG tbj
     where tbj.FEEDPROCESSDETAILID=feeddetailid and upper(ts.alias_name) = upper(tbj.DATASETNAME));
    
   END;
END SP_SAVE_IMPORT_DATATAG;

/


create or replace PROCEDURE          "SP_EXPORT_EXPORTDATASETTAG" (
sl_cursor OUT SYS_REFCURSOR
)
IS
	stm VARCHAR2(30000);

BEGIN

    stm := 'select ts.alias_name,TS.DESCRIPTION_INFO, tg.groupname,   
           tts.setname,  tf.foldername,dts.expectedresults, 
           dts.stepdesc, dts.diary,dts.sequence from T_TEST_DATASETTAG dts 
           join t_test_data_summary ts ON dts.datasetId = ts.data_summary_id
           left join t_test_group tg on tg.groupid = dts.groupid
           left join T_test_set tts on tts.setid = dts.setid
           left join t_test_folder tf on tf.folderid = dts.folderid
           order by dts.folderid, dts.sequence';
    OPEN sl_cursor FOR  stm ;

END SP_EXPORT_EXPORTDATASETTAG;

/

create or replace PROCEDURE   "SP_EXPORT_PROJ_STORYBOARD" (
PROJECT IN VARCHAR2,
sl_cursor OUT SYS_REFCURSOR)
IS


stm VARCHAR2(30000);

BEGIN
    stm := '
select  foldername, folderdesc, storyboard_name, ActionName,
 SuiteName,CaseName, DataSetName,BTEST_RESULT,BTEST_RESULT_IN_TEXT,BTEST_BEGIN_TIME,BTEST_END_TIME,
 CTEST_RESULT,CTEST_RESULT_IN_TEXT,CTEST_BEGIN_TIME,CTEST_END_TIME,BHIST_ID,CHIST_ID from (
select 
    tf.foldername,
    tf.DESCRIPTION as folderdesc,
    relprjtc.storyboard_detail_id as storyboarddetailid,
    storyboard.storyboard_id as storyboardid,
    storyboard.storyboard_name,
    prj.project_id as ProjectId,
    prj.project_name as ProjectName,
    prj.project_description as ProjectDescription,
    app.app_short_name as ApplicationName,
    suits.test_suite_id as suiteid,
    cases.test_case_id as caseid,
    dataset.data_summary_id as datasetid,
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
    relprjtc.run_order AS RunOrder,
    lkp.display_name AS ActionName,
    relprjtc.alias_name AS StepName,
    nvl(depends.alias_name,''None'') as Dependency,
     case when   HISEX.TEST_RESULT = 1 then ''PASS''
       when  HISEX.TEST_RESULT = 0 then ''FAIL''
       when  HISEX.TEST_RESULT = 2 then ''FAIL''
       Else '''' End as BTEST_RESULT,
    HISEX.TEST_RESULT_IN_TEXT as BTEST_RESULT_IN_TEXT,
    HISEX.TEST_BEGIN_TIME as BTEST_BEGIN_TIME,
    HISEX.TEST_END_TIME as BTEST_END_TIME,
    HISEX.HIST_ID as BHIST_ID,
    case when   CHISEX.TEST_RESULT = 1 then ''PASS''
       when  CHISEX.TEST_RESULT = 0 then ''FAIL''
       when  CHISEX.TEST_RESULT = 2 then ''PARTIAL''
       Else '''' End as CTEST_RESULT,
    CHISEX.TEST_RESULT_IN_TEXT as CTEST_RESULT_IN_TEXT,
    CHISEX.TEST_BEGIN_TIME as CTEST_BEGIN_TIME,
    CHISEX.TEST_END_TIME as CTEST_END_TIME,
    CHISEX.HIST_ID as CHIST_ID,
    dataset.description_info as test_step_description
    from  T_STORYBOARD_SUMMARY storyboard
INNER JOIN t_test_project prj ON storyboard.assigned_project_id = prj.project_id
INNER JOIN REL_APP_PROJ relappprj ON relappprj.project_id = prj.project_id
INNER JOIN t_registered_apps app ON app.application_id = relappprj.application_id
INNER JOIN T_PROJ_TC_MGR relprjtc ON relprjtc.project_id  = prj.project_id 
    AND relprjtc.storyboard_id = storyboard.storyboard_id 
INNER JOIN t_test_suite suits ON suits.test_suite_id = relprjtc.test_suite_id 
INNER JOIN t_test_case_summary cases ON cases.test_case_id = relprjtc.test_case_id 
left JOIN REL_TEST_CASE_TEST_SUITE  relcase ON relcase.test_suite_id = suits.test_suite_id 
    AND relcase.test_case_id = cases.test_case_id 
INNER JOIN T_STORYBOARD_DATASET_SETTING relsrt ON relsrt.storyboard_detail_id = relprjtc.storyboard_detail_id
INNER JOIN t_test_data_summary dataset ON dataset.data_summary_id = relsrt.data_summary_id 
LEFT JOIN T_TEST_DATASETTAG tdata ON TDATA.DATASETID = dataset.data_summary_id
LEFT JOIN T_TEST_FOLDER tf ON TF.FOLDERID = TDATA.FOLDERID
INNER JOIN system_lookup lkp ON lkp.value = relprjtc.run_type 
    AND lkp.field_name = ''RUN_TYPE''
    AND lkp.TABLE_NAME=''T_PROJ_TC_MGR''
LEFT JOIN T_PROJ_TC_MGR  depends ON depends.storyboard_detail_id = relprjtc.depends_on 
LEFT JOIN (
  SELECT HIS.LATEST_TEST_MARK_ID,HIS.HIST_ID,HIS.STORYBOARD_DETAIL_ID,HIS.TEST_BEGIN_TIME,HIS.TEST_CASE_ID,
         HIS.TEST_END_TIME ,HIS.TEST_RESULT_IN_TEXT ,HIS.TEST_MODE,HIS.TEST_RESULT,
         HIS.RELY_TEST_CASE_ID
  FROM T_PROJ_TEST_RESULT HIS,
  (
        select max(rslt.LATEST_TEST_MARK_ID) DASH_LATEST_MARK_ID,          
          rslt.STORYBOARD_DETAIL_ID
        from T_PROJ_TEST_RESULT rslt
        where rslt.test_mode = 1 
        group by rslt.STORYBOARD_DETAIL_ID
    ) HISMX,
    (
        select LATEST_TEST_MARK_ID,          
          rslt.STORYBOARD_DETAIL_ID,
          MAX(HIST_ID) HIST_ID_MX
        from T_PROJ_TEST_RESULT rslt
        where rslt.test_mode = 1 
group by rslt.LATEST_TEST_MARK_ID,rslt.STORYBOARD_DETAIL_ID
    ) HIS_MXID
    WHERE 
      HISMX.STORYBOARD_DETAIL_ID = HIS.STORYBOARD_DETAIL_ID
    AND HISMX.DASH_LATEST_MARK_ID = HIS.LATEST_TEST_MARK_ID 
    and HIS_MXID.LATEST_TEST_MARK_ID = HISMX.DASH_LATEST_MARK_ID
    AND HIS_MXID.STORYBOARD_DETAIL_ID = HISMX.STORYBOARD_DETAIL_ID
    AND HIS_MXID.HIST_ID_MX = HIS.HIST_ID
  ) HISEX
  ON relprjtc.STORYBOARD_DETAIL_ID=HISEX.STORYBOARD_DETAIL_ID
LEFT JOIN (
  SELECT CHIS.LATEST_TEST_MARK_ID,CHIS.HIST_ID,CHIS.STORYBOARD_DETAIL_ID,CHIS.TEST_BEGIN_TIME,CHIS.TEST_CASE_ID,
         CHIS.TEST_END_TIME ,CHIS.TEST_RESULT_IN_TEXT ,CHIS.TEST_MODE,CHIS.TEST_RESULT,
         CHIS.RELY_TEST_CASE_ID
  FROM T_PROJ_TEST_RESULT CHIS,
  (
        select max(rslt.LATEST_TEST_MARK_ID) DASH_LATEST_MARK_ID,          
          rslt.STORYBOARD_DETAIL_ID
        from T_PROJ_TEST_RESULT rslt
        where rslt.test_mode = 0 
        group by rslt.STORYBOARD_DETAIL_ID
    ) CHISMX,
    (
        select LATEST_TEST_MARK_ID,          
          rslt.STORYBOARD_DETAIL_ID,
          MAX(HIST_ID) HIST_ID_MX
        from T_PROJ_TEST_RESULT rslt
        where rslt.test_mode = 0 
      group by rslt.LATEST_TEST_MARK_ID,rslt.STORYBOARD_DETAIL_ID
    ) CHIS_MXID
    WHERE 
      CHISMX.STORYBOARD_DETAIL_ID = CHIS.STORYBOARD_DETAIL_ID
    AND CHISMX.DASH_LATEST_MARK_ID = CHIS.LATEST_TEST_MARK_ID 
    and CHIS_MXID.LATEST_TEST_MARK_ID = CHISMX.DASH_LATEST_MARK_ID
    AND CHIS_MXID.STORYBOARD_DETAIL_ID = CHISMX.STORYBOARD_DETAIL_ID
    AND CHIS_MXID.HIST_ID_MX = CHIS.HIST_ID
  ) CHISEX
  ON relprjtc.STORYBOARD_DETAIL_ID=CHISEX.STORYBOARD_DETAIL_ID  

AND relprjtc.TEST_CASE_ID=CHISEX.TEST_CASE_ID
where prj.project_name = '''|| PROJECT ||'''

ORDER BY storyboard.storyboard_name desc, relprjtc.run_order asc)
group by foldername, folderdesc, storyboard_name,ActionName,
 SuiteName,CaseName,BHIST_ID,CHIST_ID, DataSetName,BTEST_RESULT,BTEST_RESULT_IN_TEXT,BTEST_BEGIN_TIME,BTEST_END_TIME,CTEST_RESULT,CTEST_RESULT_IN_TEXT,CTEST_BEGIN_TIME,CTEST_END_TIME
 
 ';
     --edited by foram shah 25/3/19
    OPEN sl_cursor FOR  stm ;
END;

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

DBMS_OUTPUT.PUT('test' );
						---------------------------- START - CODE FOR IMPORT OBJECT FILE ---------------------------------
						--------------------------------------------------------------------------------------------------
						IF I_INDEX.FILETYPE = 'OBJECT' THEN

								DECLARE ApplicationID number:=0;
              begin

              select APPLICATION_ID into ApplicationID from (
                SELECT apps.APPLICATION_ID  FROM T_REGISTERED_APPS apps
                join TBLSTGGUIOBJECT stg on  stg.APPLICATIONNAME = apps.APP_SHORT_NAME
                where stg.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                )
              where ROWNUM = 1;



            INSERT INTO "T_OBJECT_NAMEINFO" (OBJECT_NAME_ID,OBJECT_HAPPY_NAME,PEGWINDOW_MARK,OBJNAME_DESCRIPTION)
            select SEQ_MARS_OBJECT_ID.nextval,OBJECTNAME,1,null from (
            select distinct stg.OBJECTNAME from TBLSTGGUIOBJECT stg
            left join T_OBJECT_NAMEINFO info on info.OBJECT_HAPPY_NAME = stg.OBJECTNAME
            where FEEDPROCESSDETAILID=  I_INDEX.FEEDPROCESSDETAILID and info.OBJECT_NAME_ID is null and upper(stg.Type) = 'PEGWINDOW');

            INSERT INTO "T_OBJECT_NAMEINFO" (OBJECT_NAME_ID,OBJECT_HAPPY_NAME,PEGWINDOW_MARK,OBJNAME_DESCRIPTION)
            select SEQ_MARS_OBJECT_ID.nextval,OBJECTNAME,0,null from (
            select distinct stg.OBJECTNAME from TBLSTGGUIOBJECT stg
            left join T_OBJECT_NAMEINFO info on info.OBJECT_HAPPY_NAME = stg.OBJECTNAME
            where FEEDPROCESSDETAILID=  I_INDEX.FEEDPROCESSDETAILID and info.OBJECT_NAME_ID is null  and upper(stg.Type) != 'PEGWINDOW');

            INSERT INTO "T_REGISTED_OBJECT"(OBJECT_ID,OBJECT_HAPPY_NAME,APPLICATION_ID,TYPE_ID,QUICK_ACCESS,OBJECT_TYPE,"COMMENT",
              ENUM_TYPE,OBJECT_NAME_ID,IS_CHECKERROR_OBJ,OBJ_DATA_SRC)
            SELECT  SEQ_MARS_OBJECT_ID.nextval,OBJECTNAME,ApplicationID,TYPE_ID,QUICKACCESS,PARENT,OBJECTCOMMENT ,ENUMTYPE ,OBJECT_NAME_ID,null,null from (
            select distinct stg.OBJECTNAME,guitype.TYPE_ID,stg.QUICKACCESS,stg.PARENT,stg.OBJECTCOMMENT,stg.ENUMTYPE,info.OBJECT_NAME_ID
            from TBLSTGGUIOBJECT stg
            join T_OBJECT_NAMEINFO info on upper(info.OBJECT_HAPPY_NAME) = upper(stg.OBJECTNAME)
            join T_GUI_COMPONENT_TYPE_DIC guitype on upper(stg.TYPE)= upper(guitype.TYPE_NAME)
            left join T_REGISTED_OBJECT  regObj on upper(stg.OBJECTNAME) = upper(regObj.OBJECT_HAPPY_NAME) and regObj.APPLICATION_ID = ApplicationID
            and guitype.TYPE_ID = regObj.TYPE_ID and regObj.OBJECT_TYPE = stg.PARENT
            where FEEDPROCESSDETAILID=  I_INDEX.FEEDPROCESSDETAILID and regObj.OBJECT_ID is null) ;	

           update t_object_nameinfo info set PEGWINDOW_MARK=1
  where info.object_name_id in (
            select object_name_id from   TBLSTGGUIOBJECT stg
             join T_REGISTED_OBJECT reg on info.object_name_id=reg.object_name_id
            --join T_GUI_COMPONENT_TYPE_DIC guitype on reg.type_id=guitype.type_id
            where upper(stg.parent)=upper(stg.OBJECTNAME) 
                  and stg.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID
                  and upper(INFO.OBJECT_HAPPY_NAME)=upper(stg.objectname)
                 -- and upper(guitype.type_name)='PEGWINDOW'
                 and reg.type_id=1
                  );

            UPDATE T_REGISTED_OBJECT  SET (QUICK_ACCESS,"COMMENT",ENUM_TYPE)= (
            select QUICKACCESS,OBJECTCOMMENT,ENUMTYPE from (select distinct stg.OBJECTNAME, stg.QUICKACCESS,stg.OBJECTCOMMENT,stg.ENUMTYPE
            from TBLSTGGUIOBJECT stg
            join T_OBJECT_NAMEINFO info on upper(info.OBJECT_HAPPY_NAME) = upper(stg.OBJECTNAME)
            join T_GUI_COMPONENT_TYPE_DIC guitype on upper(stg.TYPE)= upper(guitype.TYPE_NAME)
            where FEEDPROCESSDETAILID =  I_INDEX.FEEDPROCESSDETAILID and info.object_name_id = T_REGISTED_OBJECT.object_name_id and T_REGISTED_OBJECT.APPLICATION_ID = ApplicationID 
            and guitype.TYPE_ID = T_REGISTED_OBJECT.TYPE_ID and T_REGISTED_OBJECT.OBJECT_TYPE = stg.PARENT))
            WHERE EXISTS (select QUICKACCESS,OBJECTCOMMENT,ENUMTYPE from (select distinct stg.OBJECTNAME, stg.QUICKACCESS,stg.OBJECTCOMMENT,stg.ENUMTYPE
            from TBLSTGGUIOBJECT stg
            join T_OBJECT_NAMEINFO info on upper(info.OBJECT_HAPPY_NAME) = upper(stg.OBJECTNAME)
            join T_GUI_COMPONENT_TYPE_DIC guitype on upper(stg.TYPE)= upper(guitype.TYPE_NAME)
            where FEEDPROCESSDETAILID =  I_INDEX.FEEDPROCESSDETAILID and info.object_name_id = T_REGISTED_OBJECT.object_name_id and T_REGISTED_OBJECT.APPLICATION_ID = ApplicationID 
            and guitype.TYPE_ID = T_REGISTED_OBJECT.TYPE_ID and T_REGISTED_OBJECT.OBJECT_TYPE = stg.PARENT));

              end;



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
                                                        WHERE UPPER(T_TEST_SUITE.TEST_SUITE_NAME) = UPPER(tblstgtestcase.TESTSUITENAME)
                                                        AND ROWNUM=1
                                                    )
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM TEMPFALSEAPPLICATION
                                                        WHERE UPPER(tempfalseapplication.testcasename) = UPPER(tblstgtestcase.testcasename)
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
                                                                                                        WHERE UPPER(tempfalseapplication.testcasename) = UPPER(tblstgtestcase.testcasename)
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
                                                                                                WHERE UPPER(tempfalseapplication.testcasename) = UPPER(tblstgtestcase.testcasename)
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
                                                        WHERE  UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME) =  UPPER(tblstgtestcase.TESTCASENAME)
                                                        AND ROWNUM=1
                                                    )
                                                    AND NOT EXISTS (
                                                        SELECT 1
                                                        FROM TEMPFALSEAPPLICATION
                                                        WHERE  UPPER(tempfalseapplication.testcasename) =  UPPER(tblstgtestcase.testcasename)
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
                                                    WHERE UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.DATASETNAME)
                                                    AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE UPPER(tempfalseapplication.testcasename) = UPPER(tblstgtestcase.testcasename)
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
         set  tt.project_description = (select  temp.description from Storyboard_Insert_Temp temp where UPPER(temp.name) = UPPER(tt.project_name) and temp.Type = 'PROJECT' and ROWNUM = 1
)
         where Exists (select temp.name from Storyboard_Insert_Temp temp where UPPER(temp.name) = UPPER(tt.project_name) and temp.Type = 'PROJECT' and ROWNUM = 1);





            insert into t_test_project(Project_Id,project_name,project_description,creator,create_date,status)
             select T_TEST_PROJECT_SEQ.nextval,temp.name,temp.description,'System',CURRENTDATE,2 from Storyboard_Insert_Temp temp
            left join t_test_project tt on UPPER(temp.name) = UPPER(tt.project_name)
            where tt.project_id is null and temp.Type = 'PROJECT';




             insert into t_storyboard_summary(storyboard_id,assigned_project_id,storyboard_name)
             select T_TEST_STEPS_SEQ.nextval,  prj.project_id,temp.name from Storyboard_Insert_Temp temp
              join t_test_project prj on prj.project_name = temp.description 
            left join t_storyboard_summary tt on UPPER(temp.name) = UPPER(tt.storyboard_name) and prj.project_id = tt.assigned_project_id

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

 --foram start
 delete from T_STORYBOARD_DATASET_SETTING where SETTING_ID in( select distinct SETTING.SETTING_ID from T_STORYBOARD_DATASET_SETTING setting
join t_proj_tc_mgr mgr on MGR.STORYBOARD_DETAIL_ID=SETTING.STORYBOARD_DETAIL_ID
join T_TEST_PROJECT proj on MGR.PROJECT_ID=PROJ.PROJECT_ID
join T_STORYBOARD_SUMMARY ts on MGR.STORYBOARD_ID=TS.STORYBOARD_ID
join TBLSTGSTORYBOARD sa on upper(SA.PROJECTNAME)=upper(PROJ.PROJECT_NAME) and upper(SA.STORYBOARDNAME)=upper(TS.STORYBOARD_NAME)
where SA.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID and mgr.run_order not in(select runorder from TBLSTGSTORYBOARD where TBLSTGSTORYBOARD.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID));

delete from T_PROJ_TC_MGR where STORYBOARD_DETAIL_ID in
(select  distinct MGR.STORYBOARD_DETAIL_ID from t_proj_tc_mgr mgr 
join T_TEST_PROJECT proj on MGR.PROJECT_ID=PROJ.PROJECT_ID
join T_STORYBOARD_SUMMARY ts on MGR.STORYBOARD_ID=TS.STORYBOARD_ID
join TBLSTGSTORYBOARD sa on upper(SA.PROJECTNAME)=upper(PROJ.PROJECT_NAME) and upper(SA.STORYBOARDNAME)=upper(TS.STORYBOARD_NAME)
where SA.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID and mgr.run_order not in(select runorder from TBLSTGSTORYBOARD where TBLSTGSTORYBOARD.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID));

         /*delete T_STORYBOARD_DATASET_SETTING where storyboard_detail_id in
         (select mgr.storyboard_detail_id from tblstgstoryboard sa 
            join t_test_project prj  on sa.projectname = prj.project_name
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
          join T_PROJ_TC_MGR mgr on mgr.storyboard_id = tc.storyboard_id and mgr.project_id= prj.project_id
          where sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID 
         ) ;*/

         --delete from T_PROJ_TC_MGR
         /*delete T_PROJ_TC_MGR where storyboard_detail_id in
         (select mgr.storyboard_detail_id from tblstgstoryboard sa 
            join t_test_project prj  on sa.projectname = prj.project_name
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
          join T_PROJ_TC_MGR mgr on mgr.storyboard_id = tc.storyboard_id and mgr.project_id= prj.project_id
          where sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID 
         ) ;*/

  update t_proj_tc_mgr mgr set (mgr.project_id,mgr.test_case_id,mgr.storyboard_id,mgr.test_suite_id,mgr.run_type,mgr.alias_name)=

  (select  distinct prj.project_id,tcs.test_case_id,tc.storyboard_id,trs.test_suite_id,lkp.value,sa.stepname from tblstgstoryboard sa                        
            join t_test_suite trs on upper(trs.test_suite_name)= upper(sa.suitename)
            join t_test_case_summary tcs on upper(tcs.test_case_name) = upper(sa.casename)
            join t_test_project prj  on upper(sa.projectname) = upper(prj.project_name)      
            join t_storyboard_summary tc on upper(tc.storyboard_name) = upper(sa.storyboardname) and prj.project_id = tc.assigned_project_id
             join T_PROJ_TC_MGR mgr2 on mgr2.storyboard_id = tc.storyboard_id and mgr2.project_id= prj.project_id
            Inner Join system_lookup lkp on upper(lkp.display_name) = upper(sa.actionname) and lkp.field_name = 'RUN_TYPE'
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID and tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id --and trs.test_suite_id = mgr.test_suite_id 
                        --and tcs.test_case_id = mgr.test_case_id  and mgr.run_type = lkp.value 
                        and sa.runorder = mgr.run_order) 

            WHERE EXISTS ( SELECT distinct mgr2.storyboard_detail_id
            FROM TBLSTGSTORYBOARD sa
             join t_test_suite trs on upper(trs.test_suite_name) = upper(sa.suitename)
            join t_test_case_summary tcs on upper(tcs.test_case_name) = upper(sa.casename)
            join t_test_project prj  on upper(sa.projectname) = upper(prj.project_name)      
            join t_storyboard_summary tc on upper(tc.storyboard_name) = upper(sa.storyboardname) and prj.project_id = tc.assigned_project_id
             join T_PROJ_TC_MGR mgr2 on mgr2.storyboard_id = tc.storyboard_id and mgr2.project_id= prj.project_id
            Inner Join system_lookup lkp on upper(lkp.display_name) = upper(sa.actionname) and lkp.field_name = 'RUN_TYPE'
                 WHERE 
                  sa.feedprocessdetailid=I_INDEX.FEEDPROCESSDETAILID and tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id --and trs.test_suite_id = mgr.test_suite_id 
                        --and tcs.test_case_id = mgr.test_case_id  and mgr.run_type = lkp.value 
                        and sa.runorder = mgr.run_order ) ;





         /*  insert into  T_PROJ_TC_MGR(storyboard_detail_id,project_id,test_case_id,storyboard_id,run_type,run_order,test_suite_id,alias_name)
            select T_TEST_STEPS_SEQ.nextval ,prj.project_id,tcs.test_case_id,tc.storyboard_id,lkp.value,sa.runorder,trs.test_suite_id,sa.stepname from tblstgstoryboard sa                        
            join t_test_suite trs on trs.test_suite_name = sa.suitename
            join t_test_case_summary tcs on tcs.test_case_name = sa.casename
            join t_test_project prj  on sa.projectname = prj.project_name      

            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on lkp.display_name = sa.actionname and lkp.field_name = 'RUN_TYPE'
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID ;*/
--end foram
            update T_PROJ_TC_MGR mgr
            set mgr.depends_on = (select mgr2.storyboard_detail_id from tblstgstoryboard sa                        
            join t_test_suite trs on upper(trs.test_suite_name) = upper(sa.suitename)
            join t_test_case_summary tcs on upper(tcs.test_case_name) = upper(sa.casename)
            join t_test_project prj  on upper(sa.projectname) = upper(prj.project_name)     
            join t_storyboard_summary tc on upper(tc.storyboard_name) = upper(sa.storyboardname) and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on upper(lkp.display_name) = upper(sa.actionname) and lkp.field_name = 'RUN_TYPE'            
            join T_PROJ_TC_MGR mgr2 on tc.storyboard_id = mgr2.storyboard_id and prj.project_id = mgr2.project_id and mgr2.alias_name = sa.dependency
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
            and tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id and trs.test_suite_id = mgr.test_suite_id 
                        and tcs.test_case_id = mgr.test_case_id  and mgr.run_type = lkp.value and sa.runorder = mgr.run_order
            )
            where exists (select mgr2.storyboard_detail_id from tblstgstoryboard sa                        
            join t_test_suite trs on upper(trs.test_suite_name) = upper(sa.suitename)
            join t_test_case_summary tcs on upper(tcs.test_case_name) = upper(sa.casename)
            join t_test_project prj  on upper(sa.projectname) = upper(prj.project_name)     
            join t_storyboard_summary tc on upper(tc.storyboard_name) = upper(sa.storyboardname) and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on upper(lkp.display_name) = upper(sa.actionname) and lkp.field_name = 'RUN_TYPE'            
            join T_PROJ_TC_MGR mgr2 on tc.storyboard_id = mgr2.storyboard_id and prj.project_id = mgr2.project_id and mgr2.alias_name = sa.dependency
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
            and tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id and trs.test_suite_id = mgr.test_suite_id 
                        and tcs.test_case_id = mgr.test_case_id  and mgr.run_type = lkp.value and sa.runorder = mgr.run_order);            



          --foram start
          update T_STORYBOARD_DATASET_SETTING setting set(setting.data_summary_id)=
(select tr.data_summary_id from TBLSTGSTORYBOARD sa
join t_test_data_summary tr on upper(tr.alias_name) = upper(sa.datasetname)
  join t_test_suite trs on upper(trs.test_suite_name) = upper(sa.suitename)
  join t_test_case_summary tcs on upper(tcs.test_case_name) = upper(sa.casename)
  join t_test_project prj  on upper(sa.projectname) = upper(prj.project_name)  
  join t_storyboard_summary tc on upper(tc.storyboard_name) = upper(sa.storyboardname) and prj.project_id = tc.assigned_project_id
  join T_PROJ_TC_MGR mgr2 on mgr2.storyboard_id = tc.storyboard_id and mgr2.project_id= prj.project_id and sa.runorder=mgr2.run_order
 join T_STORYBOARD_DATASET_SETTING setting2 on MGR2.STORYBOARD_DETAIL_ID=SETTING2.STORYBOARD_DETAIL_ID 
where SA.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID and SETTING.STORYBOARD_DETAIL_ID=setting2.storyboard_detail_id and sa.runorder=mgr2.run_order
) where exists(
select 1 from TBLSTGSTORYBOARD sa
join t_test_data_summary tr on upper(tr.alias_name) = upper(sa.datasetname)
  join t_test_suite trs on upper(trs.test_suite_name) = upper(sa.suitename)
  join t_test_case_summary tcs on upper(tcs.test_case_name) = upper(sa.casename)
  join t_test_project prj  on upper(sa.projectname) = upper(prj.project_name)  
  join t_storyboard_summary tc on upper(tc.storyboard_name) = upper(sa.storyboardname) and prj.project_id = tc.assigned_project_id
  join T_PROJ_TC_MGR mgr2 on mgr2.storyboard_id = tc.storyboard_id and mgr2.project_id= prj.project_id and sa.runorder=mgr2.run_order
 join T_STORYBOARD_DATASET_SETTING setting2 on MGR2.STORYBOARD_DETAIL_ID=SETTING2.STORYBOARD_DETAIL_ID 
where SA.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID and SETTING.STORYBOARD_DETAIL_ID=setting2.storyboard_detail_id and sa.runorder=mgr2.run_order
);

  insert into  T_PROJ_TC_MGR(storyboard_detail_id,project_id,test_case_id,storyboard_id,run_type,run_order,test_suite_id,alias_name)
          select T_TEST_STEPS_SEQ.nextval ,prj.project_id,tcs.test_case_id,tc.storyboard_id,lkp.value,sa.runorder,trs.test_suite_id,sa.stepname from TBLSTGSTORYBOARD sa
          join t_test_suite trs on upper(trs.test_suite_name) = upper(sa.suitename)
          join t_test_case_summary tcs on upper(tcs.test_case_name) = upper(sa.casename)
          join t_test_project prj  on upper(sa.projectname) = upper(prj.project_name)      
          join t_storyboard_summary tc on upper(tc.storyboard_name) = upper(sa.storyboardname) and prj.project_id = tc.assigned_project_id
          Join system_lookup lkp on upper(lkp.display_name) = upper(sa.actionname) and lkp.field_name = 'RUN_TYPE'
          where SA.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID and not exists(
                         select 1 from T_PROJ_TC_MGR mgr where
                                  mgr.test_case_id=tcs.test_case_id and MGR.TEST_SUITE_ID=TRS.TEST_SUITE_ID and MGR.STORYBOARD_ID=tc.storyboard_id
                                 and mgr.project_id=PRJ.PROJECT_ID and sa.runorder=mgr.run_order
          );


           insert into T_STORYBOARD_DATASET_SETTING(setting_id,  storyboard_detail_id,data_summary_id)
 select T_TEST_STEPS_SEQ.nextval,MGR.STORYBOARD_DETAIL_ID, tr.data_summary_id from tblstgstoryboard sa                        
            join t_test_data_summary tr on upper(tr.alias_name) = upper(sa.datasetname)
            join t_test_suite trs on upper(trs.test_suite_name) = upper(sa.suitename)
            join t_test_case_summary tcs on upper(tcs.test_case_name) = upper(sa.casename)
            join t_test_project prj  on upper(sa.projectname) = upper(prj.project_name)
            join t_storyboard_summary tc on upper(tc.storyboard_name) = upper(sa.storyboardname) and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on upper(lkp.display_name) = upper(sa.actionname) and lkp.field_name = 'RUN_TYPE'
             join T_PROJ_TC_MGR mgr on tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id and trs.test_suite_id = mgr.test_suite_id 
                        and tcs.test_case_id = mgr.test_case_id and sa.runorder = mgr.run_order
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID and not exists(
             select 1 from T_STORYBOARD_DATASET_SETTING setting where
                                  SETTING.STORYBOARD_DETAIL_ID=MGR.STORYBOARD_DETAIL_ID and SETTING.DATA_SUMMARY_ID=TR.DATA_SUMMARY_ID  and sa.runorder=mgr.run_order
            );
            --foram end

          /* insert into  T_STORYBOARD_DATASET_SETTING(setting_id,  storyboard_detail_id,data_summary_id)
            select T_TEST_STEPS_SEQ.nextval ,mgr.storyboard_detail_id,tr.data_summary_id from tblstgstoryboard sa                        
            join t_test_data_summary tr on tr.alias_name = sa.datasetname
            join t_test_suite trs on trs.test_suite_name = sa.suitename
            join t_test_case_summary tcs on tcs.test_case_name = sa.casename
            join t_test_project prj  on sa.projectname = prj.project_name
            join t_storyboard_summary tc on tc.storyboard_name = sa.storyboardname and prj.project_id = tc.assigned_project_id
            Inner Join system_lookup lkp on lkp.display_name = sa.actionname and lkp.field_name = 'RUN_TYPE'
            join T_PROJ_TC_MGR mgr on tc.storyboard_id = mgr.storyboard_id and prj.project_id = mgr.project_id and trs.test_suite_id = mgr.test_suite_id 
                        and tcs.test_case_id = mgr.test_case_id and sa.runorder = mgr.run_order
            where  sa.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID ;*/

			UPDATE T_TEST_DATA_SUMMARY
			SET STATUS = 0 
			WHERE STATUS IS NULL ;


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
    -- EXCEPTION
   -- WHEN OTHERS THEN 
    --EXECUTE IMMEDIATE 'ALTER VIEW MV_LAST_TC_INFO COMPILE';
END;   
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_LATEST_STB_TSMOD_MARK  COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_MARS_STB_SNAPSHOT_SUB COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_STORYBOARD_LATEST COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_TC_DATASUMMARY COMPILE';
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW V_OBJECT_SNAPSHOT COMPILE';
--EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_OBJ_WITH_PEG COMPILE';

END USP_FEEDPROCESSMAPPING_Mode_D;

/

create or replace PROCEDURE             "SP_SaveTestcase" (
    FEEDPROCESSDETAIL_ID IN NUMBER,
   TESTCASEID IN NUMBER,
  RESULT OUT CLOB
)
IS
BEGIN
DECLARE
    ApplicationId number;
    CurrentDate date;
    UpdateCount number:=0;
    begin

      SELECT SYSDATE into CurrentDate  FROM DUAL ;

      delete  T_TEST_REPORT_STEPS report where STEPS_ID in
      (select stg.STEPSID from TBLSTGTESTCASESAVE stg where stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.Type = 'Delete') ;



      delete   TEST_DATA_SETTING setting where STEPS_ID in
      (select stg.STEPSID from TBLSTGTESTCASESAVE stg where stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.Type = 'Delete') ;



      delete  T_TEST_STEPS steps where STEPS_ID in 
      ( select stg.STEPSID from TBLSTGTESTCASESAVE stg where stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.Type = 'Delete');


      select min(rel.APPLICATION_ID) into ApplicationId from REL_APP_TESTCASE rel where rel.TEST_CASE_ID = TESTCASEID ;
    select count(*) into UpdateCount from TBLSTGTESTCASESAVE stg where stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.Type <> 'Delete';
if (UpdateCount > 0)
then
begin




MERGE INTO T_TEST_STEPS TTS1
  USING (
select distinct stg1.STEPSID,t.OBJECT_ID, stg1.KEY_WORD_ID,t.OBJECT_NAME_ID, stg1.PARAMETER, stg1.LCOMMENTS from (
SELECT DISTINCT stg.STEPSID, tk.KEY_WORD_ID, stg.PARAMETER, stg.LCOMMENTS, stg.OBJECT,stg.ParentObj
  FROM TBLSTGTESTCASESAVE stg
  INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
WHERE stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID     
      and stg.KEYWORD IS NOT NULL 
      and stg.Type= 'Update'  and stg.WHICHTABLE is null 
)  stg1
--CROSS JOIN TABLE(GETTOPOBJECT(stg1.OBJECT, 3)) t;
left join (select  T_REGISTED_OBJECT.OBJECT_ID,T_REGISTED_OBJECT.OBJECT_Type,T_OBJECT_NAMEINFO.OBJECT_NAME_ID ,T_OBJECT_NAMEINFO.OBJECT_HAPPY_NAME
 from T_OBJECT_NAMEINFO 
 INNER JOIN T_REGISTED_OBJECT ON T_REGISTED_OBJECT.OBJECT_NAME_ID = T_OBJECT_NAMEINFO.OBJECT_NAME_ID 
 AND T_REGISTED_OBJECT.APPLICATION_ID = ApplicationId  where  T_REGISTED_OBJECT.OBJECT_HAPPY_NAME is not null                
                  --group by T_OBJECT_NAMEINFO.OBJECT_NAME_ID ,T_OBJECT_NAMEINFO.OBJECT_HAPPY_NAME
 ) t                  
 on stg1.OBJECT = t.OBJECT_HAPPY_NAME   AND stg1.ParentObj = t.Object_Type
      
      ) TMP
   ON (TTS1.STEPS_ID = TMP.STEPSID)
   WHEN MATCHED THEN
   UPDATE SET
   TTS1.KEY_WORD_ID = TMP.KEY_WORD_ID,
   TTS1.OBJECT_ID = TMP.OBJECT_ID,
   TTS1.COLUMN_ROW_SETTING = TMP.parameter,
   TTS1.OBJECT_NAME_ID = TMP.OBJECT_NAME_ID,
   TTS1."COMMENT" = TMP.LCOMMENTS;


MERGE INTO T_TEST_STEPS TTS1
  USING (
  SELECT DISTINCT stg.STEPSID, stg.ROWNUMBER
  FROM TBLSTGTESTCASESAVE stg

    WHERE stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID     
      and stg.KEYWORD IS  NULL 
      and stg.Type= 'UpdateRowNumber'    ) TMP
   ON (TTS1.STEPS_ID = TMP.STEPSID)
   WHEN MATCHED THEN
   UPDATE SET

   TTS1.RUN_ORDER = TMP.ROWNUMBER;




DECLARE
OBJPOOLidu INT:=0;
DATASETVALUEU VARCHAR2(1000):='';
CURSOR TSHAREDOBJ
IS
  SELECT DISTINCT tsop.OBJECT_POOL_ID,stg.DATASETVALUE
  FROM TBLSTGTESTCASESAVE stg
  INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = stg.DATASETID
    AND tsop.OBJECT_ORDER = stg.ROWNUMBER
    AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
  WHERE stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.Type= 'Update'  and stg.WHICHTABLE is null 
    and stg.DATASETID > 0 and stg.DATA_SETTING_ID = 0 
    and stg.STEPSID > 0
    and stg.KEYWORD IS NOT NULL;
  BEGIN
    FOR TSHAREDOBJ_INDEX IN TSHAREDOBJ
      LOOP
          UPDATE T_SHARED_OBJECT_POOL
          SET DATA_VALUE = TSHAREDOBJ_INDEX.DATASETVALUE
          WHERE OBJECT_POOL_ID = TSHAREDOBJ_INDEX.OBJECT_POOL_ID;
      END LOOP;
END;






--UPDATE TEST_DATA_SETTING td SET (td.DATA_VALUE,td.DATA_DIRECTION) = (select distinct stg.DATASETVALUE,stg.SKIP from TBLSTGTESTCASESAVE stg 
--                                  where stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.Type= 'Update'  and stg.WHICHTABLE is null 
--                                  and stg.DATASETID > 0 and stg.DATA_SETTING_ID > 0
--                                  and td.DATA_SETTING_ID= stg.DATA_SETTING_ID)
--where 
--EXISTS (select distinct stg.DATASETVALUE,stg.SKIP from TBLSTGTESTCASESAVE stg 
--                                  where stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.Type= 'Update'  and stg.WHICHTABLE is null 
--                                  and stg.DATASETID > 0 and stg.DATA_SETTING_ID > 0
--                                  and td.DATA_SETTING_ID= stg.DATA_SETTING_ID);




MERGE INTO TEST_DATA_SETTING TTS1
  USING (
select distinct stg.DATASETVALUE,stg.SKIP ,stg.DATA_SETTING_ID
    from TBLSTGTESTCASESAVE stg 
                                  where stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.Type= 'Update' 
                                  and stg.WHICHTABLE is null 
                                  and stg.DATASETID > 0 and stg.DATA_SETTING_ID > 0
                                  
)  TMP
   ON (TTS1.DATA_SETTING_ID = TMP.DATA_SETTING_ID)
   WHEN MATCHED THEN
   UPDATE SET
   TTS1.DATA_VALUE = TMP.DATASETVALUE,
   TTS1.DATA_DIRECTION = TMP.SKIP;



INSERT INTO T_TEST_STEPS(STEPS_ID,RUN_ORDER,KEY_WORD_ID,TEST_CASE_ID,OBJECT_ID,COLUMN_ROW_SETTING,"COMMENT",OBJECT_NAME_ID)
select  T_TEST_STEPS_SEQ.nextval,ROWNUMBER,KEY_WORD_ID,TESTCASEID,object_id,PARAMETER,LCOMMENTS,object_name_id from
(select stg1.ROWNUMBER,tk.KEY_WORD_ID,stg1.TESTCASEID,t.object_id,stg1.PARAMETER,stg1.LCOMMENTS,t.object_name_id from (
SELECT distinct stg.ROWNUMBER,stg.TESTCASEID,stg.PARAMETER,stg.LCOMMENTS,stg.KEYWORD,stg.OBJECT,stg.ParentObj
from TBLSTGTESTCASESAVE stg where stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.STEPSID <= 0 )
stg1
INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg1.KEYWORD) 
 left join (select  T_REGISTED_OBJECT.OBJECT_ID,T_REGISTED_OBJECT.OBJECT_Type,T_OBJECT_NAMEINFO.OBJECT_NAME_ID ,T_OBJECT_NAMEINFO.OBJECT_HAPPY_NAME
 from T_OBJECT_NAMEINFO 
 INNER JOIN T_REGISTED_OBJECT ON T_REGISTED_OBJECT.OBJECT_NAME_ID = T_OBJECT_NAMEINFO.OBJECT_NAME_ID 
 AND T_REGISTED_OBJECT.APPLICATION_ID = ApplicationId  where  T_REGISTED_OBJECT.OBJECT_HAPPY_NAME is not null                
 ) t  on stg1.OBJECT = t.OBJECT_HAPPY_NAME   AND stg1.ParentObj = t.Object_Type 
 left join T_TEST_STEPS ts on  ts.KEY_WORD_ID = tk.key_word_id
                                                        AND ts.RUN_ORDER = stg1.rownumber
                                                        AND ts.TEST_CASE_ID = stg1.testcaseid
                                                        where ts.STEPS_ID is null
);





INSERT INTO T_SHARED_OBJECT_POOL (OBJECT_POOL_ID,DATA_SUMMARY_ID,OBJECT_NAME,OBJECT_ORDER,LOOP_ID,DATA_VALUE,CREATE_TIME,VERSION)
SELECT T_TEST_STEPS_SEQ.nextval AS ID,data_summary_id,OBJECT,ROWNUMBER,1,DATASETVALUE,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE, NULL
FROM (    
 SELECT DISTINCT stg.DATASETID as data_summary_id,stg.OBJECT,stg.ROWNUMBER,stg.DATASETVALUE
 FROM TBLSTGTESTCASESAVE stg                                           
 WHERE stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID
    and stg.KEYWORD IS NOT NULL 
    and stg.Type <> 'Delete'
    and stg.DATASETID > 0
    AND NOT EXISTS (
      SELECT 1
      FROM T_SHARED_OBJECT_POOL
      WHERE T_SHARED_OBJECT_POOL.DATA_SUMMARY_ID = stg.DATASETID
      AND NVL(T_SHARED_OBJECT_POOL.OBJECT_NAME, 'N/A') = NVL(stg.OBJECT, 'N/A')
      AND T_SHARED_OBJECT_POOL.OBJECT_ORDER = stg.ROWNUMBER
      )
ORDER BY stg.datasetid, stg.rownumber );





INSERT INTO TEST_DATA_SETTING(DATA_SETTING_ID,STEPS_ID,LOOP_ID,DATA_VALUE,DATA_SUMMARY_ID,DATA_DIRECTION,CREATE_TIME,POOL_ID)
SELECT  TEST_DATA_SETTING_SEQ.nextval ,steps_id,1,datasetvalue,data_summary_id,skip,(SELECT SYSDATE FROM DUAL),object_pool_id
FROM (    
SELECT DISTINCT t_test_steps.steps_id,stg.datasetvalue,stg.datasetid as data_summary_id,t_shared_object_pool.object_pool_id,
  stg.skip
  FROM TBLSTGTESTCASESAVE stg                                                                                      
  INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
  INNER JOIN T_SHARED_OBJECT_POOL ON T_SHARED_OBJECT_POOL.DATA_SUMMARY_ID = stg.datasetid
    AND NVL(T_SHARED_OBJECT_POOL.OBJECT_NAME,'N/A') = NVL(stg.object, 'N/A')
    AND T_SHARED_OBJECT_POOL.OBJECT_ORDER = stg.rownumber
  INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.KEY_WORD_ID = t_keyword.key_word_id
    AND T_TEST_STEPS.RUN_ORDER = stg.ROWNUMBER
    AND T_TEST_STEPS.TEST_CASE_ID = stg.TESTCASEID
  left join TEST_DATA_SETTING tds on tds.steps_id = t_test_steps.steps_id
        AND tds.DATA_SUMMARY_ID = stg.datasetid
  WHERE stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID
    and stg.KEYWORD IS NOT NULL  and stg.STEPSID <= 0
    and tds.DATA_SETTING_ID is null
    ORDER BY t_test_steps.steps_id
    
  
); 




INSERT INTO TEST_DATA_SETTING(DATA_SETTING_ID,STEPS_ID,LOOP_ID,DATA_VALUE,DATA_SUMMARY_ID,DATA_DIRECTION,CREATE_TIME,POOL_ID)
SELECT  TEST_DATA_SETTING_SEQ.nextval ,steps_id,1,datasetvalue,data_summary_id,skip,(SELECT SYSDATE FROM DUAL),object_pool_id
FROM (    
   SELECT DISTINCT t_test_steps.steps_id,stg.datasetvalue,stg.datasetid as data_summary_id,t_shared_object_pool.object_pool_id,
  stg.skip
  FROM TBLSTGTESTCASESAVE stg                                                                                      
  INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
  INNER JOIN T_SHARED_OBJECT_POOL ON T_SHARED_OBJECT_POOL.DATA_SUMMARY_ID = stg.datasetid
    AND NVL(T_SHARED_OBJECT_POOL.OBJECT_NAME,'N/A') = NVL(stg.object, 'N/A')
    AND T_SHARED_OBJECT_POOL.OBJECT_ORDER = stg.rownumber
  INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.KEY_WORD_ID = t_keyword.key_word_id
    AND T_TEST_STEPS.RUN_ORDER = stg.ROWNUMBER
    AND T_TEST_STEPS.TEST_CASE_ID = stg.TESTCASEID
  Left join TEST_DATA_SETTING tds on t_test_steps.steps_id = tds.steps_id  and stg.datasetid = tds.DATA_SUMMARY_ID 
  WHERE stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID
    and stg.KEYWORD IS NOT NULL  and stg.STEPSID > 0
    and tds.DATA_SETTING_ID is null
    ORDER BY t_test_steps.steps_id
); 

end;
end if;

    end;
BEGIN
    EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_LAST_TC_INFO COMPILE';
    EXCEPTION
    WHEN OTHERS THEN 
    EXECUTE IMMEDIATE 'ALTER VIEW MV_LAST_TC_INFO COMPILE';
END; 
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_TC_DATASUMMARY COMPILE';
end "SP_SaveTestcase";

/


create or replace procedure SP_GET_SB_All_STEPS_RESULT(
Compare_HISTIDs in clob,
Baseline_HISTIDs in clob,
--StoryboardDetailID in NUMBER,
sl_cursor OUT SYS_REFCURSOR
)
is
stm VARCHAR2(32000);
begin





stm := '';
    stm := '
  select baseline.BCount, compare.CCount,
 compare.Run_Order,
 
    baseline.test_report_Id as BaselineReportID,
   compare.test_report_Id as CompareReportID,
baseline.key_word_name,
baseline.RETURN_VALUES as Baseline_RETURN_VALUES,
compare.RETURN_VALUES as Compare_RETURN_VALUES,
COALESCE(baseline.DSCOMMENT,compare.DSCOMMENT) as "COMMENT" ,

 Case When nvl(compare.TEST_REPORT_STEP_ID,0) <= 0 and baseline.TEST_REPORT_STEP_ID > 0  Then baseline.TEST_REPORT_STEP_ID
      When compare.TEST_REPORT_STEP_ID > 0 and nvl(baseline.TEST_REPORT_STEP_ID,0) <= 0  Then compare.TEST_REPORT_STEP_ID 

      When baseline.TEST_REPORT_STEP_ID > 0 and baseline.TEST_REPORT_STEP_ID < compare.TEST_REPORT_STEP_ID  Then baseline.TEST_REPORT_STEP_ID

      When compare.TEST_REPORT_STEP_ID > 0  and compare.TEST_REPORT_STEP_ID < baseline.TEST_REPORT_STEP_ID Then compare.TEST_REPORT_STEP_ID 
            Else baseline.TEST_REPORT_STEP_ID
            End As ColumnOrder


from
(select repstep.TEST_REPORT_STEP_ID,
repstep.steps_id,
repstep.BEGIN_TIME,
repstep.END_TIME,
repstep.RUNNING_RESULT,
repstep.RETURN_VALUES,
repstep.RUNNING_RESULT_INFO,
repstep.INPUT_VALUE_SETTING,
repstep.ACTUAL_INPUT_DATA,
repstep.DATA_ORDER,
repstep.info_pic,
repstep.ADVICE,
repstep.STACKINFO,
t_keyword.key_word_name, 
relstepcomment."COMMENT",
relstepcomment.ID,
rep.test_report_Id,
step."COMMENT" as DSCOMMENT,
mgr.RUN_ORDER,
(select 
count(*) as BCount
from t_test_report rep1
inner join t_test_report_steps repstep1 on rep1.test_report_id=repstep1.test_report_id
left join t_test_steps step1 on repstep1.steps_id=step1.steps_id
left join t_keyword t_keyword1 on step1.key_word_id=t_keyword1.key_word_id
inner join t_proj_test_result prjres1 on rep1.hist_id=prjres1.hist_id
inner join T_PROJ_TC_MGR mgr1 on mgr1.STORYBOARD_DETAIL_ID = prjres1.STORYBOARD_DETAIL_ID
where prjres1.hist_id = rep.hist_id
and (t_keyword1.key_word_name in (''CaptureAndCompare'',''CaptureValue'',''CaptureAndCompareByKey'') or repstep1.steps_id is null)
and prjres1.test_mode=1-- order by mgr.RUN_ORDER
group by 
rep1.test_report_Id,
mgr1.RUN_ORDER) as BCount
from t_test_report rep
inner join t_test_report_steps repstep on rep.test_report_id=repstep.test_report_id
left join t_test_steps step on repstep.steps_id=step.steps_id
left join t_keyword on step.key_word_id=t_keyword.key_word_id
inner join t_proj_test_result prjres on rep.hist_id=prjres.hist_id
inner join T_PROJ_TC_MGR mgr on mgr.STORYBOARD_DETAIL_ID = prjres.STORYBOARD_DETAIL_ID
left join REL_TEST_REPORT_STEPS_COMMENT relstepcomment on repstep.TEST_REPORT_STEP_ID = relstepcomment.TEST_REPORT_STEP_ID
    and prjres.test_mode = relstepcomment.test_mode
where prjres.hist_id in (
   select regexp_substr('''|| Baseline_HISTIDs ||''',''[^,]+'', 1, level) from dual
    connect by regexp_substr('''|| Baseline_HISTIDs ||''',''[^,]+'', 1, level) is not null )
and (t_keyword.key_word_name in (''CaptureAndCompare'',''CaptureValue'',''CaptureAndCompareByKey'') or repstep.steps_id is null) and prjres.test_mode=1
) baseline
full outer join
(
select repstep.TEST_REPORT_STEP_ID,
repstep.steps_id,
repstep.BEGIN_TIME,
repstep.END_TIME,
repstep.RUNNING_RESULT,
repstep.RETURN_VALUES,
repstep.RUNNING_RESULT_INFO,
repstep.INPUT_VALUE_SETTING,
repstep.ACTUAL_INPUT_DATA,
repstep.DATA_ORDER,
repstep.info_pic,
repstep.ADVICE,
repstep.STACKINFO ,
relstepcomment."COMMENT",
mgr.RUN_ORDER,
relstepcomment.ID,
rep.test_report_Id,
step."COMMENT" as DSCOMMENT,
(select 
count(*) as BCount
from t_test_report rep2
inner join t_test_report_steps repstep2 on rep2.test_report_id=repstep2.test_report_id
left join t_test_steps step2 on repstep2.steps_id=step2.steps_id
left join t_keyword t_keyword2 on step2.key_word_id=t_keyword2.key_word_id
inner join t_proj_test_result prjres2 on rep2.hist_id=prjres2.hist_id
inner join T_PROJ_TC_MGR mgr2 on mgr2.STORYBOARD_DETAIL_ID = prjres2.STORYBOARD_DETAIL_ID
where prjres2.hist_id = rep.hist_id
and (t_keyword2.key_word_name in (''CaptureAndCompare'',''CaptureValue'',''CaptureAndCompareByKey'') or repstep2.steps_id is null)
and prjres2.test_mode=0-- order by mgr.RUN_ORDER
group by 
rep2.test_report_Id,
mgr2.RUN_ORDER) as CCount
from t_test_report rep
inner join t_test_report_steps repstep on rep.test_report_id=repstep.test_report_id
left join t_test_steps step on repstep.steps_id=step.steps_id
left join t_keyword on step.key_word_id=t_keyword.key_word_id
inner join t_proj_test_result prjres on rep.hist_id=prjres.hist_id
inner join T_PROJ_TC_MGR mgr on mgr.STORYBOARD_DETAIL_ID = prjres.STORYBOARD_DETAIL_ID
left join REL_TEST_REPORT_STEPS_COMMENT relstepcomment on repstep.TEST_REPORT_STEP_ID = relstepcomment.TEST_REPORT_STEP_ID and prjres.test_mode = relstepcomment.test_mode
where prjres.hist_id in  (
   select regexp_substr('''||Compare_HISTIDs||''',''[^,]+'', 1, level) from dual
    connect by regexp_substr('''||Compare_HISTIDs||''',''[^,]+'', 1, level) is not null )
and (t_keyword.key_word_name in (''CaptureAndCompare'',''CaptureValue'',''CaptureAndCompareByKey'')
or repstep.steps_id is null) and prjres.test_mode=0


) compare 
on 
nvl(baseline.input_value_setting,''null1'')=nvl(compare.input_value_setting,''null1'') and baseline.RUN_ORDER = compare.RUN_ORDER

 where nvl(baseline.input_value_setting,''null1'') != ''SKIP'' and nvl(compare.input_value_setting,''null1'') != ''SKIP''

order by ColumnOrder
';
     OPEN sl_cursor FOR  stm ; 
end;

/

create or replace PROCEDURE SP_GetTestCaseDetail( 
TESTSUITEID IN NUMBER,
TESTCASEID IN NUMBER,
DATASETID IN NUMBER,
sl_cursor OUT SYS_REFCURSOR
)
IS
stm VARCHAR2(30000);
BEGIN
    stm := '';
    stm := '
    SELECT DISTINCT
    DBMS_LOB.SUBSTR("Application",4000,1) AS "Application",
    "test_suite_name","test_case_name","test_step_description","key_word_name","object_happy_name","parameter","COMMENT",
    DBMS_LOB.SUBSTR("DATASEIDS",4000,1) AS "DATASETIDS",
    DBMS_LOB.SUBSTR("DATASETNAME",4000,1) AS "DATASETNAME",
    DBMS_LOB.SUBSTR("DATASETDESCRIPTION",4000,1) AS "DATASETDESCRIPTION",
    DBMS_LOB.SUBSTR("DATASETVALUE",4000,1) AS "DATASETVALUE",
    DBMS_LOB.SUBSTR("SKIP",4000,1) AS "SKIP",
    DBMS_LOB.SUBSTR("Data_Setting_Id",4000,1) AS "Data_Setting_Id",
    "RUN_ORDER",
    "TEST_CASE_ID",
    "STEPS_ID",
    "TEST_SUITE_ID",
    ROW_NUMBER() OVER (PARTITION BY RUN_ORDER ORDER BY RUN_ORDER) AS "ROW_NUM"
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
            REPLACE((SELECT concatenate_datasetids(tc.TEST_CASE_ID,'||DATASETID||') DATASETNAME FROM DUAL), ''\'', ''\\'') "DATASEIDS",
          --  REPLACE((SELECT concatenate_datasetnames(tc.TEST_CASE_ID) DATASETNAME FROM DUAL), ''\'', ''\\'') "DATASETNAME",
            (SELECT concatenate_datasetnames(tc.TEST_CASE_ID,'||DATASETID||') DATASETNAME FROM DUAL) "DATASETNAME",
            REPLACE((SELECT concatenate_datasetdescription(tc.TEST_CASE_ID,'||DATASETID||') FROM DUAL), ''\'', ''\\'') "DATASETDESCRIPTION",
         --   REPLACE((SELECT concatenate_teststepvalue(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL), ''\'', ''\\'') "DATASETVALUE",--old code
            --  REPLACE((SELECT concatenate_teststepdatavalue(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL), ''\'', ''\\'') "DATASETVALUE",--new code
            (SELECT concatenate_teststepdatavalue(tc.TEST_CASE_ID, ts.STEPS_ID,'||DATASETID||') FROM DUAL) "DATASETVALUE",
            (SELECT concatenate_teststepskip(tc.TEST_CASE_ID, ts.STEPS_ID,'||DATASETID||') FROM DUAL) "SKIP",
            (SELECT concatenate_teststepdatasetId(tc.TEST_CASE_ID, ts.STEPS_ID,'||DATASETID||') FROM DUAL) "Data_Setting_Id",
            ts."RUN_ORDER",
            tc."TEST_CASE_ID",
            ts."STEPS_ID",
            t."TEST_SUITE_ID"
        FROM T_TEST_SUITE t
        INNER JOIN REL_TEST_CASE_TEST_SUITE reltcts ON reltcts.TEST_SUITE_ID = t.TEST_SUITE_ID
        INNER JOIN T_TEST_CASE_SUMMARY tc ON tc.TEST_CASE_ID = reltcts.TEST_CASE_ID
        LEFT JOIN T_TEST_STEPS ts ON ts.TEST_CASE_ID = tc.TEST_CASE_ID
        LEFT JOIN T_KEYWORD tk ON tk.KEY_WORD_ID = ts.KEY_WORD_ID
        LEFT JOIN T_OBJECT_NAMEINFO tobn ON tobn.OBJECT_NAME_ID = ts.OBJECT_NAME_ID  
        where reltcts.test_case_id =  '|| TESTCASEID ||' and reltcts.test_suite_id = '|| TESTSUITEID ||' )


    ';




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
            ( SELECT concatenate_datasetnames(tc.TEST_CASE_ID,0) DATASETNAME FROM DUAL) "DATASETNAME",
            (SELECT concatenate_datasetdescription(tc.TEST_CASE_ID,0) FROM DUAL) "DATASETDESCRIPTION",
            (SELECT concatenate_teststepvalue(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "DATASETVALUE",
            (SELECT concatenate_teststepskip(tc.TEST_CASE_ID, ts.STEPS_ID,0) FROM DUAL) "SKIP",
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
select LISTAGG("Application", '','') WITHIN GROUP (ORDER BY "Application") as "Application","test_suite_name","test_case_name",
       "test_step_description","key_word_name","object_happy_name"
,"parameter", "COMMENT","DATASETNAME", "DATASETDESCRIPTION", "DATASETVALUE","SKIP","RUN_ORDER"  from
(
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
            tapp.app_short_name  AS "Application",           
            t.TEST_SUITE_NAME AS "test_suite_name",
            tc.TEST_CASE_NAME as "test_case_name",
            tc.TEST_STEP_DESCRIPTION AS "test_step_description",
            tk.KEY_WORD_NAME as "key_word_name",
            tobn.OBJECT_HAPPY_NAME AS "object_happy_name",
            ts.COLUMN_ROW_SETTING as "parameter",
            ts."COMMENT",
            ( SELECT concatenate_datasetnames(tc.TEST_CASE_ID,0) DATASETNAME FROM DUAL) "DATASETNAME",
            (SELECT concatenate_datasetdescription(tc.TEST_CASE_ID,0) FROM DUAL) "DATASETDESCRIPTION",
            (SELECT concatenate_teststepvalue(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "DATASETVALUE",
            (SELECT concatenate_teststepskip(tc.TEST_CASE_ID, ts.STEPS_ID,0) FROM DUAL) "SKIP",
            ts."RUN_ORDER"
          FROM T_TEST_SUITE t
          INNER JOIN REL_TEST_CASE_TEST_SUITE reltcts ON reltcts.TEST_SUITE_ID = t.TEST_SUITE_ID
          INNER JOIN REL_APP_TESTCASE reapp ON reapp.test_case_id = reltcts.TEST_CASE_ID 
          INNER JOIN T_REGISTERED_APPS tapp ON tapp.APPLICATION_ID = reapp.APPLICATION_ID
          INNER JOIN T_TEST_CASE_SUMMARY tc ON tc.TEST_CASE_ID = reltcts.TEST_CASE_ID
          INNER JOIN T_TEST_STEPS ts ON ts.TEST_CASE_ID = tc.TEST_CASE_ID
          INNER JOIN T_KEYWORD tk ON tk.KEY_WORD_ID = ts.KEY_WORD_ID
          LEFT JOIN T_OBJECT_NAMEINFO tobn ON tobn.OBJECT_NAME_ID = ts.OBJECT_NAME_ID
          WHERE t.TEST_SUITE_NAME='''||TESTSUITENAME||'''
           ) )
           group by "test_suite_name","test_case_name",
       "test_step_description","key_word_name","object_happy_name"
,"parameter", "COMMENT","DATASETNAME", "DATASETDESCRIPTION", "DATASETVALUE","SKIP","RUN_ORDER" 
           ORDER BY 3 DESC,13';
    OPEN sl_cursor FOR  stm ; 

    INSERT INTO logreport(logreport.filename,logreport.object,logreport.status,logreport.logdetails,logreport.id,logreport.createdon,logreport.feedprocessdetailid)
    select 'Test Suite Name : ' || TESTSUITENAME AS filename,'' AS object,'SUCCESS','TEST CASE NAME :'|| t_test_case_summary.test_case_name || ' DATASET COUNT : ' || (SELECT COUNT(1) FROM rel_tc_data_summary  where rel_tc_data_summary.TEST_CASE_ID = T_TEST_CASE_SUMMARY.TEST_CASE_ID)|| ' DATASET COUNT : '||  (SELECT COUNT(1) FROM t_test_steps where t_test_steps.test_case_id = T_TEST_CASE_SUMMARY.TEST_CASE_ID),LOGREPORT_SEQ.NEXTVAL,(SELECT SYSDATE FROM DUAL),0 FROM t_test_suite  INNER JOIN REL_TEST_CASE_TEST_SUITE on rel_test_case_test_suite.TEST_SUITE_ID = t_test_suite.TEST_SUITE_ID INNER JOIN T_TEST_CASE_SUMMARY ON T_TEST_CASE_SUMMARY.TEST_CASE_ID = REL_TEST_CASE_TEST_SUITE.TEST_CASE_ID  where UPPER(test_suite_name) =  UPPER(TESTSUITENAME);

END;

/

CREATE OR REPLACE NONEDITIONABLE PROCEDURE "SP_GET_DBCONNECTIONBYID" (
CONNECTION_ID IN NUMBER,
sl_cursor OUT SYS_REFCURSOR
)
IS 
stm VARCHAR2(30000);

BEGIN
stm := 'select con.DBCONNECTION_ID, nvl(con.DATABASENAME, '''') as DATABASENAME, 
con.DATABASE_VALUE as BLOBValuestr from T_DBCONNECTION con
where con.DBCONNECTION_ID =' ||CONNECTION_ID;
--stm := 'select con.DATABASE_VALUE as BLOBValuestr from T_DBCONNECTION con where conn_id =' ||CONNECTION_ID;
OPEN sl_cursor FOR  stm;
END SP_GET_DBCONNECTIONBYID;

/

CREATE OR REPLACE NONEDITIONABLE PROCEDURE "SP_GET_DBCONNECTION" (
sl_cursor OUT SYS_REFCURSOR
)
IS 
stm VARCHAR2(30000);

BEGIN

stm := 'select con.DBCONNECTION_ID, nvl(con.DATABASENAME, '''') as DATABASENAME, nvl(con.SCHEMA, '''') as SCHEMA, con.DATABASE_VALUE as BLOBValuestr from T_DBCONNECTION con';
OPEN sl_cursor FOR  stm;
END SP_GET_DBCONNECTION;