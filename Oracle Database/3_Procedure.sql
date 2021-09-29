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
 
 create or replace PROCEDURE SP_LIST_KEYWORDS(
Startrec in number,
totalpagesize in number,
ColumnName in varchar2,
Columnorder in varchar2,
NameSearch in varchar2,
TypeSearch in varchar2,
EntryInFIle in varchar2,
sl_cursor OUT SYS_REFCURSOR
)
IS
stm VARCHAR2(30000);
begin
DECLARE 
    Columnnamevalue varchar2(200):=NULL;
    WhereClause varchar2(2000):=Null;
    NewColumnName varchar(200):=Null;

Begin
    if lower(ColumnName) = 'name'
    then begin NewColumnName := 'Upper(keywordname)';
    end;
    end if;

    if lower(ColumnName) = 'entryfile'
    then begin NewColumnName := 'Upper(entry)';
    end;
    end if;

    if lower(ColumnName) = 'controltype'
    then begin NewColumnName := 'dbms_lob.substr(upper(typename), dbms_lob.getlength(Upper(typename)), 1)';
    end;
    end if;

    WhereClause:='keyword.key_word_name is not null';
    IF NameSearch is not null
    then begin Columnnamevalue:='keyword.key_word_name';
        WhereClause:=' (lower(keyword.key_word_name) like lower(''%'||NameSearch||'%''))';
    end;
    end IF;



    IF TypeSearch is not null
    then begin Columnnamevalue:='gui.type_name';
     if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:=WhereClause || '  (lower(gui.type_name) like lower(''%'||TypeSearch||'%''))';
    end;
    end IF;

    IF EntryInFIle is not null
    then begin Columnnamevalue:='keyword.entry_in_data_file';
     if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:=WhereClause || '   (lower(keyword.entry_in_data_file) like lower(''%'||EntryInFIle||'%''))';
    end;
    end IF;

 if WhereClause is not null
 then begin 
 WhereClause:= ' where ' || WhereClause;
 end;
 end if;

stm := '';
stm:='
select * from (Select RESULT_COUNT, row_number() over (order by '|| NewColumnName ||' '||ColumnOrder||') as row_num, keywordid,keywordname,entry,typename,typeid from(
select  COUNT(*) OVER () RESULT_COUNT,

keyword.key_word_id as keywordid,
keyword.key_word_name as keywordname,
keyword.entry_in_data_file as entry,
(SELECT  concatenate_keyword_type(keyword.key_word_id) FROM DUAL) AS typename,
(SELECT  concatenate_keyword_typeid(keyword.key_word_id) FROM DUAL) AS typeid
 from t_keyword keyword
 left join T_DIC_RELATION_KEYWORD relkeyword on keyword.key_word_id=relkeyword.key_word_id
 left Join T_GUI_COMPONENT_TYPE_DIC gui On relkeyword.type_id = gui.type_id
  '||whereclause||'

 group by keyword.key_word_id, keyword.key_word_name,keyword.entry_in_data_file


)order by '|| NewColumnName ||' '||ColumnOrder||'
)where row_num between '||Startrec||' and '|| (Startrec-1+totalpagesize) ||'
';
 DBMS_OUTPUT.PUT_LINE(stm); 
OPEN sl_cursor FOR  stm ;
end;
end SP_LIST_KEYWORDS;

 /
 
 create or replace PROCEDURE Sp_List_Objects(
Startrec in number,
totalpagesize in number,
ColumnName in varchar2,
Columnorder in varchar2,
objectname in varchar2,
typename in varchar2,
quickaccess in varchar2,
parent in varchar2,
APPLICATION IN VARCHAR2,
sl_cursor OUT SYS_REFCURSOR
)
IS

stm VARCHAR2(30000);
BEGIN
DECLARE 
    Columnnamevalue varchar2(200):=NULL;
Begin
IF ColumnName='Object Name'
    then begin Columnnamevalue:='ni.object_happy_name';
    end;
    end IF;
    IF ColumnName='Type'
    then begin Columnnamevalue:='typed.TYPE_NAME';
    end;
    end IF;
    IF ColumnName='Internal Access'
    then begin Columnnamevalue:='o.QUICK_ACCESS'; 
    end;
    end IF;
     IF ColumnName='Parent Pegwindow'
    then begin Columnnamevalue:='o.OBJECT_TYPE'; 
    end;
    end IF;

stm := '';
stm :='
select RESULT_COUNT,row_num,objectid,OBJECTNAME, TYPE,QuickAccess,PARENT,ENUM_TYPE,applicationid,description,checkerror,SQL from(
SELECT COUNT(*) OVER () RESULT_COUNT, row_number() over (order by Upper('|| Columnnamevalue ||') '||ColumnOrder||') as row_num, ni.OBJECT_NAME_ID as objectid, ni.object_happy_name as OBJECTNAME,
                   typed.TYPE_NAME as TYPE, 
                   o.QUICK_ACCESS as QuickAccess,
                   o.OBJECT_TYPE as PARENT,
                  -- o.COMMENT as objectcomment,
                   o.ENUM_TYPE,
                   o.APPLICATION_ID as applicationid,
                   ni.OBJNAME_DESCRIPTION as description,
                   o.IS_CHECKERROR_OBJ as checkerror,
                   '''' as SQL
            FROM T_OBJECT_NAMEINFO ni
            INNER JOIN T_REGISTED_OBJECT o ON o.OBJECT_NAME_ID = ni.OBJECT_NAME_ID
            INNER JOIN T_GUI_COMPONENT_TYPE_DIC typed ON o.TYPE_ID=typed.TYPE_ID
            WHERE APPLICATION_ID='||APPLICATION||'
                AND EXISTS (
                    SELECT 1
                    FROM t_object_nameinfo tobjparent
                    WHERE tobjparent.object_happy_name = o.object_type
                )
                and (lower(ni.object_happy_name) like lower(''%'||objectname||'%''))
                and (lower(typed.TYPE_NAME) like lower(''%'||typename||'%''))
                and (lower(o.OBJECT_TYPE) like lower(''%'||parent||'%''))
                and (lower(o.QUICK_ACCESS) like lower(''%'||quickaccess||'%''))
                    ORDER BY Upper('|| Columnnamevalue ||') '||ColumnOrder||'
                )
                where row_num between '||Startrec||' and '|| (Startrec-1+totalpagesize) ||' 
            ';
OPEN sl_cursor FOR  stm ;
end;
End;

 /
 create or replace PROCEDURE SP_LIST_PROJECTS(
Startrec in number,
totalpagesize in number,
ColumnName in varchar2,
Columnorder in varchar2,
NameSearch in varchar2,
DescriptionSearch in varchar2,
AppSearch in varchar2,
StatusSearch in varchar2,
sl_cursor OUT SYS_REFCURSOR
)
IS
stm VARCHAR2(30000);
begin
DECLARE 
    Columnnamevalue varchar2(200):=NULL;
    WhereClause varchar2(2000):=Null;
    NewColumnName varchar(200):=Null;
Begin
    if lower(ColumnName) = 'name'
    then begin NewColumnName := 'Upper(projectname)';
    end;
    end if;

    if lower(ColumnName) = 'description'
    then begin NewColumnName := 'Upper(description)';
    end;
    end if;

    if lower(ColumnName) = 'application'
    then begin NewColumnName := 'dbms_lob.substr(Upper(Applicationame), dbms_lob.getlength(Upper(Applicationame)), 1)';
    end;
    end if;

    if lower(ColumnName) = 'status'
    then begin NewColumnName := 'projectstatus';
    end;
    end if;
    WhereClause:='projectname is not null';
    IF NameSearch is not null
    then begin Columnnamevalue:='projectname';
        WhereClause:=' (lower(projectname) like lower(''%'||NameSearch||'%''))';
    end;
    end IF;

    IF DescriptionSearch is not null
    then begin Columnnamevalue:='description';
        if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:= WhereClause || '  (lower(description) like lower(''%'||DescriptionSearch||'%''))';
    end;
    end IF;

    IF AppSearch is not null
    then begin Columnnamevalue:='Applicationame';
     if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:=WhereClause || '  (lower(Applicationame) like lower(''%'||AppSearch||'%''))';
    end;
    end IF;

    IF StatusSearch is not null
    then begin Columnnamevalue:='projectstatus';
     if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:=WhereClause || '   (lower(projectstatus) like lower(''%'||StatusSearch||'%''))';
    end;
    end IF;

 if WhereClause is not null
 then begin 
 WhereClause:= ' where ' || WhereClause;
 end;
 end if;

stm := '';
stm:='
select * from (Select COUNT(*) OVER () RESULT_COUNT, row_number() over (order by '|| NewColumnName ||' '||ColumnOrder||')  as row_num, projectid,projectname,description,applicationid,Applicationame,projectstatus,statusid from(
select  
proj.project_id as projectid,
proj.project_name as projectname,
case when proj.project_description is null then ''''
else proj.project_description end
as description,
(SELECT  concatenate_appid_project(proj.project_id) FROM DUAL) AS applicationid,
(SELECT  concatenate_app_project(proj.project_id) FROM DUAL) AS Applicationame,
case when proj.status =''1'' then ''Edit''
else ''Ready to test'' end as projectstatus,
proj.status as statusid
 from t_test_project proj
 left join rel_app_proj reltapp on proj.project_id=reltapp.project_id
left join t_registered_apps app on reltapp.application_id=app.application_id

--where proj.project_name is not null --and '||whereclause||'
--and (lower(proj.project_name) like lower(''%'||namesearch||'%''))
--and (lower(proj.project_description) like lower(''%'||descriptionsearch||'%''))
--and (lower(app.app_short_name) like lower(''%'||appsearch||'%''))
                --and (proj.status='||statussearch||')

 group by proj.project_id,proj.project_name,proj.project_description,proj.status

)'||whereclause||' order by '|| NewColumnName ||' '||ColumnOrder||'
)
where row_num between '||Startrec||' and '|| (Startrec-1+totalpagesize) ||'

';
 DBMS_OUTPUT.PUT_LINE(stm); 
OPEN sl_cursor FOR  stm ;
end;
end SP_LIST_PROJECTS;

 /

create or replace PROCEDURE Sp_List_Storyboards(
Startrec in number,
totalpagesize in number,
ColumnName in varchar2,
Columnorder in varchar2,
SName in varchar2,
SDesc in varchar2,
Projname in varchar2,
sl_cursor OUT SYS_REFCURSOR
)
IS

stm VARCHAR2(30000);

BEGIN
DECLARE 
    Columnnamevalue varchar2(200):=NULL;
Begin
    IF ColumnName='Name'
    then begin Columnnamevalue:='tss.storyboard_name';
    end;
    end IF;
    IF ColumnName='Description'
    then begin Columnnamevalue:='tss.description';
    end;
    end IF;
    IF ColumnName='Project Name'
    then begin Columnnamevalue:='ttp.project_name'; 
    end;
    end IF;
stm := '';
stm :='
select RESULT_COUNT, row_num,StoryboardId,Storyboardname ,description,ProjectId,ProjectName from
(
select COUNT(*) OVER () RESULT_COUNT, row_number() over (order by Upper('|| Columnnamevalue ||') '||ColumnOrder||') as row_num,tss.storyboard_id as StoryboardId, tss.storyboard_name as Storyboardname,tss.description as description,ttp.project_id as ProjectId, ttp.project_name as ProjectName from t_storyboard_summary tss
inner join t_test_project ttp on tss.assigned_project_id=ttp.project_id
where (lower(tss.storyboard_name) like lower(''%'||SName||'%''))
and (lower(tss.description) like lower(''%'||SDesc||'%''))
and (lower(ttp.project_name) like lower(''%'||Projname||'%''))
order by Upper('|| Columnnamevalue ||') '||ColumnOrder||'
) 
where
row_num between '|| Startrec ||' and '|| (Startrec-1+totalpagesize) ||' 

';
OPEN sl_cursor FOR  stm ;
end;
END Sp_List_Storyboards;


 /
create or replace PROCEDURE SP_LIST_TESTCASES(
Startrec in number,
totalpagesize in number,
ColumnName in varchar2,
Columnorder in varchar2,
NameSearch in varchar2,
DescriptionSearch in varchar2,
AppSearch in varchar2,
SuiteSearch in varchar2,
sl_cursor OUT SYS_REFCURSOR
)
IS
stm VARCHAR2(30000);
begin
DECLARE 
    Columnnamevalue varchar2(200):=NULL;
    WhereClause varchar2(2000):=Null;
    NewColumnName varchar(200):=Null;
Begin
    if lower(ColumnName) = 'name'
    then begin NewColumnName := 'Upper(casename)';
    end;
    end if;

    if lower(ColumnName) = 'description'
    then begin NewColumnName := 'Upper(description)';
    end;
    end if;

    if lower(ColumnName) = 'application'
    then begin NewColumnName := 'dbms_lob.substr(upper(Applicationame), dbms_lob.getlength(Upper(Applicationame)), 1)';
    end;
    end if;

    if lower(ColumnName) = 'testsuiteid'
    then begin NewColumnName := 'dbms_lob.substr(upper(Suitename), dbms_lob.getlength(Upper(Suitename)), 1)';
    end;
    end if;

    IF NameSearch is not null
    then begin Columnnamevalue:='t.test_case_name';
        WhereClause:=' (lower(t.test_case_name) like lower(''%'||NameSearch||'%''))';
    end;
    end IF;

    IF DescriptionSearch is not null
    then begin Columnnamevalue:='t.test_step_description';
        if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:= WhereClause || '  (lower(t.test_step_description) like lower(''%'||DescriptionSearch||'%''))';
    end;
    end IF;

    IF AppSearch is not null
    then begin Columnnamevalue:='app.app_short_name';
     if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:=WhereClause || '  (lower(app.app_short_name) like lower(''%'||AppSearch||'%''))';
    end;
    end IF;

    IF SuiteSearch is not null
    then begin Columnnamevalue:='suite.test_suite_name';
     if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:=WhereClause || '   (lower(suite.test_suite_name) like lower(''%'||SuiteSearch||'%''))';
    end;
    end IF;

 if WhereClause is not null
 then begin 
 WhereClause:= ' where ' || WhereClause;
 end;
 end if;

stm := '';
stm :='
select * from (
Select RESULT_COUNT, row_number() over (order by '|| NewColumnName ||' '||ColumnOrder||') as row_num, testcaseid,casename,description,suiteid,applicationid,Applicationame,Suitename from(
select  
COUNT(*) OVER () RESULT_COUNT,
t.test_case_id as testcaseid,
t.test_case_name as casename,
--t.test_step_description as description,
case when t.test_step_description is null then ''''
else t.test_step_description end
as description,
--suite.test_suite_id as suiteid,
--app.application_id as applicationid,
(SELECT  CONCATENATE_APPID_TESTCASE(t.test_case_id) FROM DUAL) AS applicationid,
(SELECT  CONCATENATE_APP_TESTCASE(t.test_case_id) FROM DUAL) AS Applicationame,
(SELECT  CONCATENATE_TSID_TC(t.test_case_id) FROM DUAL) AS suiteid,
(SELECT  CONCATENATE_TS_TC(t.test_case_id) FROM DUAL) AS Suitename

from t_test_case_summary t
left join rel_app_testcase relapp on t.test_case_id=relapp.test_case_id
left join t_registered_apps app on relapp.application_id=app.application_id
left join rel_test_case_test_suite reltcts on t.test_case_id=reltcts.test_case_id
left join t_test_suite suite on reltcts.test_suite_id=suite.test_suite_id
 '||  WhereClause  ||'
group by t.test_case_id,t.test_case_name,t.test_step_description
)
order by '|| NewColumnName ||' '||ColumnOrder||')
where row_num between '||Startrec||' and '|| (Startrec-1+totalpagesize) ||'
';
 --DBMS_OUTPUT.PUT_LINE(stm); 
OPEN sl_cursor FOR  stm ;
end;
end SP_LIST_TESTCASES;

/

create or replace PROCEDURE SP_LIST_TESTSUITES(
Startrec in number,
totalpagesize in number,
ColumnName in varchar2,
Columnorder in varchar2,
NameSearch in varchar2,
DescriptionSearch in varchar2,
AppSearch in varchar2,
ProjectSearch in varchar2,
sl_cursor OUT SYS_REFCURSOR
)
IS
stm VARCHAR2(30000);
begin
DECLARE 
    Columnnamevalue varchar2(200):=NULL;
    WhereClause varchar2(2000):=Null;
    NewColumnName varchar(200):=Null;
Begin
    if lower(ColumnName) = 'name'
    then begin NewColumnName := 'Upper(suitename)';
    end;
    end if;

    if lower(ColumnName) = 'description'
    then begin NewColumnName := 'Upper(description)';
    end;
    end if;

    if lower(ColumnName) = 'application'
    then begin NewColumnName := 'dbms_lob.substr(upper(Applicationame), dbms_lob.getlength(Upper(Applicationame)), 1)';
    end;
    end if;

    if lower(ColumnName) = 'project'
    then begin NewColumnName := 'dbms_lob.substr(upper(projectname), dbms_lob.getlength(Upper(projectname)), 1)';
    end;
    end if;

    IF NameSearch is not null
    then begin Columnnamevalue:='suite.test_suite_name';
        WhereClause:=' (lower(suite.test_suite_name) like lower(''%'||NameSearch||'%''))';
    end;
    end IF;

    IF DescriptionSearch is not null
    then begin Columnnamevalue:='suite.test_suite_description';
        if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:= WhereClause || '  (lower(suite.test_suite_description) like lower(''%'||DescriptionSearch||'%''))';
    end;
    end IF;

    IF AppSearch is not null
    then begin Columnnamevalue:='app.app_short_name';
     if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:=WhereClause || '  (lower(app.app_short_name) like lower(''%'||AppSearch||'%''))';
    end;
    end IF;

    IF ProjectSearch is not null
    then begin Columnnamevalue:='proj.project_name';
     if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:=WhereClause || '   (lower(proj.project_name) like lower(''%'||ProjectSearch||'%''))';
    end;
    end IF;

 if WhereClause is not null
 then begin 
 WhereClause:= ' where ' || WhereClause;
 end;
 end if;

stm := '';
stm :='
select * from (
Select RESULT_COUNT, row_number() over (order by '|| NewColumnName ||' '||ColumnOrder||' ) as row_num, testsuiteid,suitename,description,projectid,applicationid,Applicationame,projectname from(
select  
COUNT(*) OVER () RESULT_COUNT,
suite.test_suite_id as testsuiteid,
suite.test_suite_name as suitename,
case when suite.test_suite_description is null then ''''
else suite.test_suite_description end
as description,
(SELECT  CONCATENATE_APPID_TESTSUITE(suite.test_suite_id) FROM DUAL) AS applicationid,
(SELECT  CONCATENATE_APP_TESTSUITE(suite.test_suite_id) FROM DUAL) AS Applicationame,
(SELECT  CONCATENATE_PROJECTID_TS(suite.test_suite_id) FROM DUAL) AS projectid,
(SELECT  CONCATENATE_PROJECT_TS(suite.test_suite_id) FROM DUAL) AS projectname
 from t_test_suite suite
 left join rel_app_testsuite reltapp on suite.test_suite_id=reltapp.test_suite_id
left join t_registered_apps app on reltapp.application_id=app.application_id
left join rel_test_suit_project relprj on suite.test_suite_id=relprj.test_suite_id
left join t_test_project proj on relprj.project_id=proj.project_id
 '||  WhereClause  ||'
 group by suite.test_suite_id,suite.test_suite_name,suite.test_suite_description
)
order by '|| NewColumnName ||' '||ColumnOrder||'
)
where row_num between '||Startrec||' and '|| (Startrec-1+totalpagesize) ||'

';
 DBMS_OUTPUT.PUT_LINE(stm); 
OPEN sl_cursor FOR  stm ;
end;
end SP_LIST_TESTSUITES;

 / 
 create or replace PROCEDURE   "SP_GET_VARIABLE_DETAILS"(
Startrec in number,
totalpagesize in number,
ColumnName in varchar2,
Columnorder in varchar2,
FieldNameSearch in varchar2,
tablesearch in varchar2,
displaynamesearch in varchar2,
statussearch in varchar2,
sl_cursor OUT SYS_REFCURSOR)
IS

stm VARCHAR2(30000);

BEGIN
DECLARE 
    Columnnamevalue varchar2(200):=NULL;
     WhereClause varchar2(2000):=Null;
Begin
    IF ColumnName='Name'
    then begin Columnnamevalue:='Name';
    end;
    end IF;
    IF ColumnName='Type'
    then begin Columnnamevalue:='Type';
    end;
    end IF;
    IF ColumnName='Value'
    then begin Columnnamevalue:='Value'; 
    end;
    end IF;
    IF ColumnName='Status'
    then begin Columnnamevalue:='Base';
    end;
    end IF;
     WhereClause:='Name is not null';
    IF FieldNameSearch is not null
    then begin Columnnamevalue:='Name';
        WhereClause:=' (lower(Name) like lower(''%'||FieldNameSearch||'%''))';
    end;
    end IF;

     IF tablesearch is not null
    then begin Columnnamevalue:='Type';
        if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:= WhereClause || '  (lower(Type) like lower(''%'||tablesearch||'%''))';
    end;
    end IF;
     IF displaynamesearch is not null
    then begin Columnnamevalue:='Value';
        if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:= WhereClause || '  (lower(Value) like lower(''%'||displaynamesearch||'%''))';
    end;
    end IF;
     IF statussearch is not null
    then begin Columnnamevalue:='Base';
        if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:= WhereClause || '  (lower(Base) like lower(''%'||statussearch||'%''))';
    end;
    end IF;

if WhereClause is not null
 then begin 
 WhereClause:= ' where ' || WhereClause;
 end;
 end if;

stm := '';
stm :='
select * from(
select  COUNT(*) OVER () RESULT_COUNT,row_number() over (order by Upper('|| Columnnamevalue ||') '||ColumnOrder||') as row_num,lookupid, Name,Value,Type,Base from(
select a.lookup_id as lookupid, a.Field_Name as Name
,case when a.display_name is null then '''' 
    else a.display_name end 
    as Value
,a.table_name as Type 
, case when  a.status = 1 and a.table_name in (''MODAL_VAR'',''LOOP_VAR'') then ''BASELINE''
       when a.status = 2 and a.table_name in (''MODAL_VAR'',''LOOP_VAR'') then ''COMPARE''
       Else '''' End
AS Base
from system_lookup a
where 
a.table_name 
in 
(''MODAL_VAR'',''GLOBAL_VAR'',''LOOP_VAR'') 
)
'||whereclause||'
--where (lower(Name) like lower(''%'||FieldNameSearch||'%''))
--and (Value is null or lower(Value) like lower(''%'||displaynamesearch||'%''))
--and (Type is null or lower(Type) like lower(''%'||tablesearch||'%''))
--and (Base is null or lower(Base) like lower(''%'||statussearch||'%''))
order by Upper('|| Columnnamevalue ||') '||ColumnOrder||'


--and (lower(Base) like ''%'||statussearch||'%'')

)where
row_num between '|| Startrec ||' and '|| (Startrec-1+totalpagesize) ||'

';
OPEN sl_cursor FOR  stm ;
end;
END SP_GET_VARIABLE_DETAILS;

 /


create or replace procedure SP_Get_LeftPanelProjectList(
UserId in number,
sl_cursor OUT SYS_REFCURSOR
)
is 
stm VARCHAR2(30000);
begin

 

stm := '';
stm :='
select proj.PROJECT_ID,info.TESTER_ID, PROJECT_NAME ,PROJECT_DESCRIPTION,
( select count(test_suite_id) from ( select reltsp.test_suite_id,ttp.project_id from REL_TEST_SUIT_PROJECT reltsp
--join REL_TEST_CASE_TEST_SUITE reltstc on reltsp.test_suite_id = reltstc.test_suite_id
--join REL_TC_DATA_SUMMARY reldata on reltstc.test_case_id = reldata.test_case_id
join T_TEST_PROJECT ttp on reltsp.project_id = ttp.project_id

 

group by reltsp.test_suite_id,ttp.project_id
) 
where project_id = proj.PROJECT_ID
) as TestSuiteCount,
( select count( STORYBOARD_NAME) from T_STORYBOARD_SUMMARY tss
 where tss.ASSIGNED_PROJECT_ID = proj.PROJECT_ID and tss.STORYBOARD_NAME is not null  

 

) as StoryBoardCount

 

from T_TEST_PROJECT proj
join REL_PROJECT_USER relPR on proj.PROJECT_ID = relpr.project_id
join T_TESTER_INFO info on relpr.user_id = info.TESTER_ID

 

where TESTER_ID = ' || UserId || '

 

';
OPEN sl_cursor FOR  stm ;
END SP_Get_LeftPanelProjectList;
/

create or replace procedure SP_Get_Storyboard_details(
PROJECTID in number,
Storyboardid in number,
sl_cursor OUT SYS_REFCURSOR
)
is 
stm VARCHAR2(30000);
begin

 

stm := '';
stm :='
  select  LISTAGG(ApplicationName) WITHIN GROUP (ORDER BY ApplicationName) as ApplicationName,RunOrder,ProjectId,ProjectName,ProjectDescription,storyboardid,storyboarddetailid,storyboard_name,ActionName
,StepName,suiteid,caseid,datasetid, SuiteName,CaseName, DataSetName, Dependency,BTEST_RESULT,BTEST_RESULT_IN_TEXT,BTEST_BEGIN_TIME,BTEST_END_TIME,BHIST_ID,CTEST_RESULT,CTEST_RESULT_IN_TEXT,CTEST_BEGIN_TIME,CTEST_END_TIME,CHIST_ID,test_step_description  from (
select  
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
    relprjtc.run_order AS RunOrder,--,relprjtc.run_order AS RunOrder,
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
--INNER JOIN REL_TC_DATA_SUMMARY reldataset ON reldataset.test_case_id = cases.test_case_id
INNER JOIN T_STORYBOARD_DATASET_SETTING relsrt ON relsrt.storyboard_detail_id = relprjtc.storyboard_detail_id
INNER JOIN t_test_data_summary dataset ON --dataset.data_summary_id = reldataset.data_summary_id AND
dataset.data_summary_id = relsrt.data_summary_id 
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
where storyboard.storyboard_id='||Storyboardid||' and prj.project_id='||PROJECTID||'

 

ORDER BY storyboard.storyboard_name desc, relprjtc.run_order asc)
group by RunOrder,ProjectName,ProjectId,ProjectDescription,storyboardid,storyboarddetailid,storyboard_name,ActionName
,StepName, suiteid,caseid,datasetid,SuiteName,CaseName, DataSetName, Dependency,BTEST_RESULT,BTEST_RESULT_IN_TEXT,BTEST_BEGIN_TIME,BTEST_END_TIME,BHIST_ID,CTEST_RESULT,CTEST_RESULT_IN_TEXT,CTEST_BEGIN_TIME,CTEST_END_TIME,CHIST_ID,test_step_description
';
OPEN sl_cursor FOR  stm ;
END SP_Get_Storyboard_details;
/

create or replace PROCEDURE SP_GetTestCaseDetail( 
TESTSUITEID IN NUMBER,
TESTCASEID IN NUMBER,
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
            REPLACE((SELECT concatenate_datasetids(tc.TEST_CASE_ID) DATASETNAME FROM DUAL), ''\'', ''\\'') "DATASEIDS",
          --  REPLACE((SELECT concatenate_datasetnames(tc.TEST_CASE_ID) DATASETNAME FROM DUAL), ''\'', ''\\'') "DATASETNAME",
            (SELECT concatenate_datasetnames(tc.TEST_CASE_ID) DATASETNAME FROM DUAL) "DATASETNAME",
            REPLACE((SELECT concatenate_datasetdescription(tc.TEST_CASE_ID) FROM DUAL), ''\'', ''\\'') "DATASETDESCRIPTION",
         --   REPLACE((SELECT concatenate_teststepvalue(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL), ''\'', ''\\'') "DATASETVALUE",--old code
            --  REPLACE((SELECT concatenate_teststepdatavalue(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL), ''\'', ''\\'') "DATASETVALUE",--new code
            (SELECT concatenate_teststepdatavalue(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "DATASETVALUE",
            (SELECT concatenate_teststepskip(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "SKIP",
            (SELECT concatenate_teststepdatasetId(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "Data_Setting_Id",
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
 create or replace PROCEDURE   "SP_COPY_SELECTED_OBJECTS" (
    Objectid in varchar,
    OLDAPPID IN LONG,
   NEWAPPID IN LONG,
    RESULT OUT CLOB
)
IS 
BEGIN
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
select SEQ_MARS_OBJECT_ID.nextval,t1.OBJECT_HAPPY_NAME,newappid--new objectid
,t1.TYPE_ID,t1.QUICK_ACCESS,t1.OBJECT_TYPE,t1."COMMENT",t1.ENUM_TYPE,t1.OBJECT_NAME_ID,t1.IS_CHECKERROR_OBJ,t1.OBJ_DATA_SRC
from T_OBJECT_NAMEINFO t
join T_REGISTED_OBJECT t1 on t.OBJECT_NAME_ID = t1.OBJECT_NAME_ID
left join T_REGISTED_OBJECT fromobj on fromobj.OBJECT_HAPPY_NAME = t1.OBJECT_HAPPY_NAME and fromobj.OBJECT_TYPE = t1.OBJECT_TYPE and
    fromobj.TYPE_ID = t1.TYPE_ID and fromobj.APPLICATION_ID = newappid--new objectid
where t1.application_id = oldappid and t1.object_name_id in (select regexp_substr(Objectid,'[^,]+', 1, level) from dual
             connect by regexp_substr(Objectid, '[^,]+', 1, level) is not null) --old objectid
and fromobj.application_id is null;
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW V_OBJECT_SNAPSHOT COMPILE';
end;

/

create or replace PROCEDURE   "SP_COPYDATASETS" (
    OLDDATASETID IN LONG,
   NEWDATASETID IN LONG
   -- RESULT OUT CLOB
)
IS 
BEGIN
insert into test_data_setting(data_setting_id,
                               steps_id,
                               loop_id,
                               data_value,
                               value_or_object,
                               description,
                               data_summary_id,
                               data_direction,
                               version,
                               create_time,
                               pool_id)
    select TEST_DATA_SETTING_SEQ.nextval,
           t.steps_id,
           t.loop_id,
           t.data_value,
           t.value_or_object,
           t.description,
           newdatasetid,
           t.data_direction,
           t.version,
           sysdate,
           t.pool_id 
           from test_data_setting t
            left join test_data_setting t1 on t1.data_summary_id=newdatasetid
           where t.data_summary_id=olddatasetid and t1.data_summary_id is null;
EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_TC_DATASUMMARY COMPILE';
end;
/

create or replace PROCEDURE   "SP_COPYOBJECTS" (
    OLDAPPID IN LONG,
   NEWAPPID IN LONG,
    RESULT OUT CLOB
)
IS 
BEGIN
--insert into "t_registed_object"(object_id,object_happy_name,application_id,type_id,quick_access,object_type,"comment",enum_type,object_name_id,is_checkerror_obj,obj_data_src)
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
select SEQ_MARS_OBJECT_ID.nextval,t1.OBJECT_HAPPY_NAME,newappid--new objectid
,t1.TYPE_ID,t1.QUICK_ACCESS,t1.OBJECT_TYPE,t1."COMMENT",t1.ENUM_TYPE,t1.OBJECT_NAME_ID,t1.IS_CHECKERROR_OBJ,t1.OBJ_DATA_SRC
from T_OBJECT_NAMEINFO t
join T_REGISTED_OBJECT t1 on t.OBJECT_NAME_ID = t1.OBJECT_NAME_ID
left join T_REGISTED_OBJECT fromobj on fromobj.OBJECT_HAPPY_NAME = t1.OBJECT_HAPPY_NAME and fromobj.OBJECT_TYPE = t1.OBJECT_TYPE and
    fromobj.TYPE_ID = t1.TYPE_ID and fromobj.APPLICATION_ID = newappid--new objectid
where t1.application_id = oldappid --old objectid
and fromobj.application_id is null;

EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW V_OBJECT_SNAPSHOT COMPILE';
end;

/

create or replace PROCEDURE          "SP_EXPORT_ALL_STORYBOARDS" (
sl_cursor OUT SYS_REFCURSOR)
IS

stm VARCHAR2(30000);

BEGIN
    stm := ' select RunOrder,ApplicationName,ProjectName,ProjectDescription,storyboard_name,ActionName,StepName,SuiteName,CaseName,DataSetName,Dependency from (
select 
ROW_NUMBER() OVER (PARTITION BY RunOrder,ApplicationName,ProjectName,ProjectDescription,storyboard_name,ActionName,StepName ORDER BY RunOrder DESC ) as lRowId
,RunOrder,ApplicationName,ProjectName,ProjectDescription,storyboard_name,ActionName,StepName,SuiteName,CaseName,DataSetName,Dependency from (
select  LISTAGG(ApplicationName) WITHIN GROUP (ORDER BY ApplicationName) as ApplicationName,RunOrder,ProjectName,ProjectDescription,storyboard_name,ActionName
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
ORDER BY storyboard.storyboard_name desc, relprjtc.run_order asc)
group by RunOrder,ProjectName,ProjectDescription,storyboard_name,ActionName
,StepName, SuiteName,CaseName, DataSetName, Dependency)
group by RunOrder,ApplicationName,ProjectName,ProjectDescription,storyboard_name,ActionName,StepName,SuiteName,CaseName,DataSetName,Dependency) where lrowid = 1
 ';
            --edited by foram shah 25/3/19
    OPEN sl_cursor FOR  stm ;
END;

/

create or replace PROCEDURE SP_EXPORT_ALL_TESTSUITES( 
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
           ) 
           ORDER BY 3 DESC,13';
    OPEN sl_cursor FOR  stm ; 
END;

/

create or replace PROCEDURE   "SP_EXPORT_STORYBOARD" (
PROJECT IN VARCHAR2,
sl_cursor OUT SYS_REFCURSOR)
IS


stm VARCHAR2(30000);

BEGIN
    stm := '
select  LISTAGG(ApplicationName,'','') WITHIN GROUP (ORDER BY ApplicationName) as ApplicationName,RunOrder,ProjectName,ProjectDescription,storyboard_name,ActionName
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
left JOIN T_PROJ_TC_MGR relprjtc ON relprjtc.project_id  = prj.project_id 
	AND relprjtc.storyboard_id = storyboard.storyboard_id
left JOIN t_test_suite suits ON suits.test_suite_id = relprjtc.test_suite_id
left JOIN t_test_case_summary cases ON cases.test_case_id = relprjtc.test_case_id
left JOIN REL_TEST_CASE_TEST_SUITE  relcase ON relcase.test_suite_id = suits.test_suite_id 
	AND relcase.test_case_id = cases.test_case_id
--INNER JOIN REL_TC_DATA_SUMMARY reldataset ON reldataset.test_case_id = cases.test_case_id
left JOIN T_STORYBOARD_DATASET_SETTING relsrt ON relsrt.storyboard_detail_id = relprjtc.storyboard_detail_id
left JOIN t_test_data_summary dataset ON --dataset.data_summary_id = reldataset.data_summary_id 
 dataset.data_summary_id = relsrt.data_summary_id
left JOIN system_lookup lkp ON lkp.value = relprjtc.run_type 
	AND lkp.field_name = ''RUN_TYPE''
	AND lkp.TABLE_NAME=''T_PROJ_TC_MGR''
LEFT JOIN T_PROJ_TC_MGR  depends ON depends.storyboard_detail_id = relprjtc.depends_on
WHERE prj.project_name = '''|| PROJECT ||'''

ORDER BY storyboard.storyboard_name desc, relprjtc.run_order asc)
group by RunOrder,ProjectName,ProjectDescription,storyboard_name,ActionName
,StepName, SuiteName,CaseName, DataSetName, Dependency

    ';
            --edited by foram shah 25/3/19
    OPEN sl_cursor FOR  stm ;
END;
/
create or replace PROCEDURE   "SP_EXPORT_STORYBOARDNEW" (
PROJECT IN VARCHAR2,
Storyboardname IN VARCHAR2,
sl_cursor OUT SYS_REFCURSOR)
IS


stm VARCHAR2(30000);

BEGIN
    stm := '
    select storyboarddetailid,storyboardid,ProjectId, LISTAGG(ApplicationName,'','') WITHIN GROUP (ORDER BY ApplicationName) as ApplicationName,RunOrder,ProjectName,ProjectDescription,storyboard_name,ActionName
,StepName, SuiteName,CaseName, DataSetName, Dependency,test_step_description  from (
SELECT distinct
    relprjtc.storyboard_detail_id as storyboarddetailid,
     storyboard.storyboard_id as storyboardid,
     prj.project_id as ProjectId,
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
	nvl(depends.alias_name,''None'') as Dependency,
    dataset.description_info as test_step_description
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
--INNER JOIN REL_TC_DATA_SUMMARY reldataset ON reldataset.test_case_id = cases.test_case_id
INNER JOIN T_STORYBOARD_DATASET_SETTING relsrt ON relsrt.storyboard_detail_id = relprjtc.storyboard_detail_id
INNER JOIN t_test_data_summary dataset ON --dataset.data_summary_id = reldataset.data_summary_id 
 dataset.data_summary_id = relsrt.data_summary_id
INNER JOIN system_lookup lkp ON lkp.value = relprjtc.run_type 
	AND lkp.field_name = ''RUN_TYPE''
	AND lkp.TABLE_NAME=''T_PROJ_TC_MGR''
LEFT JOIN T_PROJ_TC_MGR  depends ON depends.storyboard_detail_id = relprjtc.depends_on
WHERE prj.project_name = '''|| PROJECT ||''' and storyboard.storyboard_name='''||Storyboardname||'''

ORDER BY storyboard.storyboard_name desc, relprjtc.run_order asc)
group by RunOrder,ProjectName,ProjectId,ProjectDescription,storyboardid,storyboarddetailid,storyboard_name,ActionName
,StepName, SuiteName,CaseName, DataSetName, Dependency,test_step_description
';
            --edited by foram shah 25/3/19
    OPEN sl_cursor FOR  stm ;
END;

/

create or replace PROCEDURE   "SP_EXPORT_STORYBOARDNEW" (
PROJECT IN VARCHAR2,
Storyboardname IN VARCHAR2,
sl_cursor OUT SYS_REFCURSOR)
IS


stm VARCHAR2(30000);

BEGIN
    stm := '
    select storyboarddetailid,storyboardid,ProjectId, LISTAGG(ApplicationName,'','') WITHIN GROUP (ORDER BY ApplicationName) as ApplicationName,RunOrder,ProjectName,ProjectDescription,storyboard_name,ActionName
,StepName, SuiteName,CaseName, DataSetName, Dependency,test_step_description  from (
SELECT distinct
    relprjtc.storyboard_detail_id as storyboarddetailid,
     storyboard.storyboard_id as storyboardid,
     prj.project_id as ProjectId,
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
	nvl(depends.alias_name,''None'') as Dependency,
    dataset.description_info as test_step_description
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
--INNER JOIN REL_TC_DATA_SUMMARY reldataset ON reldataset.test_case_id = cases.test_case_id
INNER JOIN T_STORYBOARD_DATASET_SETTING relsrt ON relsrt.storyboard_detail_id = relprjtc.storyboard_detail_id
INNER JOIN t_test_data_summary dataset ON --dataset.data_summary_id = reldataset.data_summary_id 
 dataset.data_summary_id = relsrt.data_summary_id
INNER JOIN system_lookup lkp ON lkp.value = relprjtc.run_type 
	AND lkp.field_name = ''RUN_TYPE''
	AND lkp.TABLE_NAME=''T_PROJ_TC_MGR''
LEFT JOIN T_PROJ_TC_MGR  depends ON depends.storyboard_detail_id = relprjtc.depends_on
WHERE prj.project_name = '''|| PROJECT ||''' and storyboard.storyboard_name='''||Storyboardname||'''

ORDER BY storyboard.storyboard_name desc, relprjtc.run_order asc)
group by RunOrder,ProjectName,ProjectId,ProjectDescription,storyboardid,storyboarddetailid,storyboard_name,ActionName
,StepName, SuiteName,CaseName, DataSetName, Dependency,test_step_description
';
            --edited by foram shah 25/3/19
    OPEN sl_cursor FOR  stm ;
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
            ( SELECT concatenate_datasetnames(tc.TEST_CASE_ID) DATASETNAME FROM DUAL) "DATASETNAME",
            (SELECT concatenate_datasetdescription(tc.TEST_CASE_ID) FROM DUAL) "DATASETDESCRIPTION",
            (SELECT concatenate_teststepvalue(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "DATASETVALUE",
            (SELECT concatenate_teststepskip(tc.TEST_CASE_ID, ts.STEPS_ID) FROM DUAL) "SKIP",
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

create or replace PROCEDURE   "SP_VALIDATEOBJECTS" (
    OLDAPPID IN LONG,
   NEWAPPID IN LONG,
sl_cursor OUT SYS_REFCURSOR
)
is 
stm VARCHAR2(30000);
begin

stm := '';
stm :='
select objectname from(
select t1.OBJECT_HAPPY_NAME as objectname
from T_OBJECT_NAMEINFO t
join T_REGISTED_OBJECT t1 on t.OBJECT_NAME_ID = t1.OBJECT_NAME_ID
left join T_REGISTED_OBJECT fromobj on fromobj.OBJECT_HAPPY_NAME = t1.OBJECT_HAPPY_NAME and fromobj.OBJECT_TYPE = t1.OBJECT_TYPE and
    fromobj.TYPE_ID = t1.TYPE_ID and fromobj.APPLICATION_ID = '|| NEWAPPID || '
where t1.application_id = '||OLDAPPID ||'--old objectid
and fromobj.application_id is not null)

';
OPEN sl_cursor FOR  stm ;

END;

/
create or replace PROCEDURE             "USP_FEEDPROCESSMAPPING_WEB_D" (
    FEEDPROCESSID1 IN NUMBER,
   ISOVERWRITE IN NUMBER,
    RESULT OUT CLOB
)
IS 
BEGIN
declare
CURSOR FORTBLFEEDPROCESS IS 
		    SELECT  FEEDPROCESSDETAILID,FEEDPROCESSID,FILENAME,FEEDPROCESSSTATUS,CREATEDBY,CREATEDON,FILETYPE FROM TBLFEEDPROCESSDETAILS WHERE FEEDPROCESSID = FEEDPROCESSID1;
BEGIN
			--OPEN FORTBLFEEDPROCESS;

			FOR I_INDEX IN FORTBLFEEDPROCESS
			LOOP



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

                                    DELETE FROM tblstgwebtestcase
                                    WHERE ID IN (
                                        SELECT DISTINCT ID
                                        from tblstgwebtestcase t
                                        where t.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                        AND EXISTS (
                                            SELECT 1
                                            FROM tblstgwebtestcase s
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

									SELECT COUNT(*) INTO COUNT_APPLICATIONID FROM T_REGISTERED_APPS,tblstgwebtestcase WHERE  
									T_REGISTERED_APPS.APP_SHORT_NAME= tblstgwebtestcase.APPLICATION
									AND tblstgwebtestcase.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

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
                                                        FROM tblstgwebtestcase 
                                                        WHERE tblstgwebtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
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
                                            					SELECT DISTINCT upper(tblstgwebtestcase.TESTSUITENAME) INTO TESTSUITENAME
					                                        FROM tblstgwebtestcase
                                            					WHERE tblstgwebtestcase.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID
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
                                                        SELECT DISTINCT Application, tblstgwebtestcase.testcasename
                                                        FROM tblstgwebtestcase 
                                                        WHERE tblstgwebtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
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
											SELECT TESTSUITENAME FROM tblstgwebtestcase  WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID GROUP BY  TESTSUITENAME;

											SELECT COUNT(*) INTO TESTSUITECOUNT FROM TEMP1;

							IF TESTSUITECOUNT = 1 THEN -- IF - 2 -START
									DECLARE
											AUTO_TEMP1 NUMBER:=0;
											AUTO_TEMPDWTESTCASE NUMBER:=0;  -- UNDER OBSERVATION
											AUTO_TEMPSTGTESTCASE NUMBER:=0; -- UNDER OBSERVATION
											CURRENTDATE TIMESTAMP;

									BEGIN
											SELECT SYSDATE INTO CURRENTDATE FROM DUAL;
											DELETE FROM TEMP1;
											SELECT  TEMPSTGTESTCASE_SEQ.NEXTVAL INTO AUTO_TEMPSTGTESTCASE FROM DUAL;
											INSERT INTO TEMPSTGTESTCASE(TESTCASENAME,DATASETNAME,KEYWORDNAME,OBJECTNAME,ORDERNUMBER) 
											SELECT DISTINCT TESTCASENAME,DATASETNAME,KEYWORD,OBJECT,ROWNUMBER FROM tblstgwebtestcase WHERE FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;
											SELECT  TEMPDWTESTCASE_SEQ.NEXTVAL INTO AUTO_TEMPDWTESTCASE FROM DUAL;
                                            INSERT INTO TEMPDWTESTCASE(TESTCASENAME,DATASETNAME,KEYWORDNAME,OBJECTNAME,ORDERNUMBER) 
                                            SELECT DISTINCT
                                                T_TEST_CASE_SUMMARY.TEST_CASE_NAME,
                                                DATASETNAME,T_KEYWORD.KEY_WORD_NAME,tblstgwebtestcase.OBJECT,tblstgwebtestcase.ROWNUMBER 
                                            FROM tblstgwebtestcase
                                            INNER JOIN T_TEST_CASE_SUMMARY ON UPPER(T_TEST_CASE_SUMMARY.TEST_CASE_NAME)=UPPER(tblstgwebtestcase.TESTCASENAME)
                                            INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME)  =UPPER(tblstgwebtestcase.KEYWORD)
                                            WHERE tblstgwebtestcase.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID;

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
                                --dbms_output.put_line('All Update Started');
                                SELECT CURRENT_TIMESTAMP INTO CURRENTTIME FROM DUAL;

                                -- Update T_TEST_STEPS - Start



                                            MERGE INTO T_TEST_STEPS TTS1
                                        USING (
                                            SELECT DISTINCT tts.STEPS_ID,t.OBJECT_ID, tk.KEY_WORD_ID,t.OBJECT_NAME_ID, stg.parameter, stg.COMMENTS
                                            FROM tblstgwebtestcase stg
                                           -- INNER JOIN t_test_case_summary tcs ON UPPER(tcs.test_case_name) = UPPER(stg.testcasename)
                                            INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
                                            INNER JOIN T_TEST_STEPS tts ON tts.steps_id = stg.stepsid
                                                AND tts.RUN_ORDER = stg.ROWNUMBER
                                            --    AND tts.TEST_CASE_ID = tcs.test_case_id
                                            CROSS JOIN TABLE(GETTOPOBJECT(stg.object, APPLICATIONID)) t
                                            WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND stg.DATASETMODE IS NULL
                                                 and stg.KEYWORD IS NOT NULL 
                                        ) TMP
                                        ON (TTS1.STEPS_ID = TMP.STEPS_ID)
                                        WHEN MATCHED THEN
                                        UPDATE SET
                                            TTS1.KEY_WORD_ID = TMP.KEY_WORD_ID,
                                            TTS1.OBJECT_ID = TMP.OBJECT_ID,
                                            TTS1.COLUMN_ROW_SETTING = TMP.parameter,
                                            TTS1.OBJECT_NAME_ID = TMP.OBJECT_NAME_ID,
                                            TTS1."COMMENT" = TMP.COMMENTS;

                                -- Update T_TEST_STEPS - End

                                -- Update T_SHARED_OBJECT_POOL - Start


                                        DECLARE
                                          OBJPOOLidu INT:=0;
                                          DATASETVALUEU VARCHAR2(1000):='';
                                          CURSOR TSHAREDOBJ
                                          IS
                                          SELECT DISTINCT tsop.OBJECT_POOL_ID,stg.DATASETVALUE
                                          FROM tblstgwebtestcase stg

                                          INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = stg.datasetid
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

                                -- Update T_SHARED_OBJECT_POOL - Start

                                -- Update TEST_DATA_SETTING - Start


                                       --date 2020 02 12 cherish
                                       MERGE INTO  TEST_DATA_SETTING tdss

                                       USING (                                         
                                          SELECT DISTINCT tdss.DATA_SETTING_ID,tdss.DATA_VALUE,stg.DATASETVALUE, tdss.POOL_ID,tsop.OBJECT_POOL_ID,tdss.DATA_DIRECTION,stg.skip
                                          FROM tblstgwebtestcase stg                                                                                                                         
                                          INNER JOIN T_SHARED_OBJECT_POOL tsop ON tsop.DATA_SUMMARY_ID = stg.datasetid
                                              AND tsop.OBJECT_ORDER = stg.ROWNUMBER
                                              AND NVL(tsop.OBJECT_NAME,'N/A') = NVL(stg.OBJECT,'N/A')
                                          INNER JOIN T_TEST_STEPS tts ON tts.steps_id = stg.stepsid
                                              AND tts.RUN_ORDER = stg.ROWNUMBER
                                          INNER JOIN TEST_DATA_SETTING tdss ON tdss.data_setting_id = stg.data_setting_id                                             
                                          WHERE stg.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                              AND stg.DATASETMODE IS NULL
                                               and stg.KEYWORD IS NOT NULL 
                                        ) TMP
                                        ON (tdss.DATA_SETTING_ID = TMP.DATA_SETTING_ID)
                                        WHEN MATCHED THEN
                                        UPDATE SET
                                            tdss.DATA_VALUE = TMP.DATASETVALUE,
                                            tdss.POOL_ID = TMP.POOL_ID,
                                            tdss.DATA_DIRECTION = TMP.skip;


                                  MERGE INTO t_test_data_summary TTS1
                                  USING (
                                      SELECT DISTINCT tcs.DATA_SUMMARY_ID, stg.DATASETDESCRIPTION
                                      FROM tblstgwebtestcase stg
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

                                -- UPDATE t_test_data_summary -END
 SELECT T_TEST_STEPS_SEQ.nextval INTO MAXRow FROM DUAL;

                                        INSERT INTO T_TEST_STEPS(STEPS_ID,RUN_ORDER,KEY_WORD_ID,TEST_CASE_ID,OBJECT_ID,COLUMN_ROW_SETTING,VALUE_SETTING,"COMMENT",IS_RUNNABLE,OBJECT_NAME_ID)
                                        SELECT T_TEST_STEPS_SEQ.nextval  AS ID,rownumber,key_word_id,test_case_id,object_id,parameter,NULL,comments,NULL,object_name_id
                                        FROM (    
                                            SELECT DISTINCT tblstgwebtestcase.rownumber,t_keyword.key_word_id,tblstgwebtestcase.testcaseid as TEST_CASE_ID,t.object_id,tblstgwebtestcase.parameter,tblstgwebtestcase.comments,t.object_name_id
                                            FROM tblstgwebtestcase                                                                                      
                                            INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgwebtestcase.KEYWORD)
                                            CROSS JOIN TABLE(GETTOPOBJECT(tblstgwebtestcase.OBJECT, APPLICATIONID)) t
                                            WHERE tblstgwebtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgwebtestcase.DATASETMODE IS NULL 
                                                 and tblstgwebtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM T_TEST_STEPS
                                                    WHERE T_TEST_STEPS.KEY_WORD_ID = t_keyword.key_word_id
                                                        AND T_TEST_STEPS.RUN_ORDER = tblstgwebtestcase.rownumber
                                                        AND T_TEST_STEPS.TEST_CASE_ID = tblstgwebtestcase.testcaseid
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgwebtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                            ORDER BY tblstgwebtestcase.testcaseid,tblstgwebtestcase.rownumber      
                                        ); 

                              INSERT INTO T_SHARED_OBJECT_POOL (OBJECT_POOL_ID,DATA_SUMMARY_ID,OBJECT_NAME,OBJECT_ORDER,LOOP_ID,DATA_VALUE,CREATE_TIME,VERSION)
                                        SELECT T_TEST_STEPS_SEQ.nextval AS ID,data_summary_id,object,rownumber,1,datasetvalue,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE, NULL
                                        FROM (    
                                            SELECT DISTINCT tblstgwebtestcase.datasetid as data_summary_id,tblstgwebtestcase.object,tblstgwebtestcase.rownumber,tblstgwebtestcase.datasetvalue
                                            FROM tblstgwebtestcase                                           
                                            WHERE tblstgwebtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgwebtestcase.DATASETMODE IS NULL 
                                                 and tblstgwebtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM T_SHARED_OBJECT_POOL
                                                    WHERE T_SHARED_OBJECT_POOL.DATA_SUMMARY_ID = tblstgwebtestcase.datasetid
                                                        AND NVL(T_SHARED_OBJECT_POOL.OBJECT_NAME, 'N/A') = NVL(tblstgwebtestcase.object, 'N/A')
                                                        AND T_SHARED_OBJECT_POOL.OBJECT_ORDER = tblstgwebtestcase.rownumber

                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgwebtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                                ORDER BY tblstgwebtestcase.datasetid, tblstgwebtestcase.rownumber
                                        ); 

                                        INSERT INTO TEST_DATA_SETTING(DATA_SETTING_ID,STEPS_ID,LOOP_ID,DATA_VALUE,VALUE_OR_OBJECT,DESCRIPTION,DATA_SUMMARY_ID,DATA_DIRECTION,VERSION,CREATE_TIME,POOL_ID)
                                        SELECT  TEST_DATA_SETTING_SEQ.nextval AS ID,steps_id,1,datasetvalue,NULL,NULL,data_summary_id,skip,NULL,(SELECT SYSDATE FROM DUAL) AS CURRENTDATE,object_pool_id
                                        FROM (    
                                            SELECT DISTINCT t_test_steps.steps_id,tblstgwebtestcase.datasetvalue,tblstgwebtestcase.datasetid as data_summary_id,t_shared_object_pool.object_pool_id,tblstgwebtestcase.skip
                                            FROM tblstgwebtestcase                                                                                      
                                            INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgwebtestcase.KEYWORD)
                                            INNER JOIN T_SHARED_OBJECT_POOL ON T_SHARED_OBJECT_POOL.DATA_SUMMARY_ID = tblstgwebtestcase.datasetid
                                                AND NVL(T_SHARED_OBJECT_POOL.OBJECT_NAME,'N/A') = NVL(tblstgwebtestcase.object, 'N/A')
                                                AND T_SHARED_OBJECT_POOL.OBJECT_ORDER = tblstgwebtestcase.rownumber
                                            INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.KEY_WORD_ID = t_keyword.key_word_id
                                                AND T_TEST_STEPS.RUN_ORDER = tblstgwebtestcase.rownumber
                                                AND T_TEST_STEPS.TEST_CASE_ID = tblstgwebtestcase.testcaseid
                                            WHERE tblstgwebtestcase.feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
                                                AND tblstgwebtestcase.DATASETMODE IS NULL 
                                                 and tblstgwebtestcase.KEYWORD IS NOT NULL 
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEST_DATA_SETTING
                                                    WHERE test_data_setting.steps_id = t_test_steps.steps_id
                                                        AND TEST_DATA_SETTING.DATA_SUMMARY_ID = tblstgwebtestcase.datasetid
                                                        AND ROWNUM=1
                                                )
                                                AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM TEMPFALSEAPPLICATION
                                                    WHERE tempfalseapplication.testcasename = tblstgwebtestcase.testcasename
                                                        AND tempfalseapplication.FLAG = 0
                                                        AND ROWNUM=1
                                                )
                                                ORDER BY t_test_steps.steps_id
                                        ); 

                                --dbms_output.put_line(CURRENTTIME);
                            END;    

                        -- Code added by Shivam - To save the errorlog for tab with wrong application - Start
                            execute immediate 'TRUNCATE TABLE TEMP1';
                            execute immediate 'TRUNCATE TABLE TEMPSTGTESTCASE';
                            execute immediate 'TRUNCATE TABLE TEMPDWTESTCASE';

---------------------------------------------------------------------------------------------------------- LOGIC FOR NOT OVERWRITE - END ----------------------------------------------------------------------------------------------------------

							END;

						END IF; -- IF - 2 - END

									END;

								END IF;

							END;
						END IF;





------------------------------------------------------- END - CODE FOR IMPORT TEST CASE ---------------------------------
--------------------------------------------------------------------------------------------------------------------------


			END LOOP;
END;
--execute immediate 'TRUNCATE TABLE tblstgwebtestcase';
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
--EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_OBJ_WITH_PEG COMPILE';

END USP_FEEDPROCESSMAPPING_WEB_D;

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


create or replace PROCEDURE SP_GetTestCaseValidationResult( 
FEEDPROCESSID1 IN NUMBER,
sl_cursor OUT SYS_REFCURSOR
)
IS
stm VARCHAR2(30000);
BEGIN
stm := '';
    stm := ' select distinct  ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID,FEEDPROCESSDETAILID from TBLSTGTESTCASEVALIDATION where
    FEEDPROCESSID = '||  FEEDPROCESSID1 ||'';
      OPEN sl_cursor FOR  stm ; 


END;
/
create or replace PROCEDURE             "SP_CheckValidationTestCase" (
    FEEDPROCESSID1 IN NUMBER,
   
  RESULT OUT CLOB
)
IS
BEGIN
DECLARE
            TypeID number;
            FirstPegWindowId number:=-1;
            PegCount number:= 0;
  BEGIN
	 select TYPE_ID into TypeID from T_GUI_COMPONENT_TYPE_DIC where UPPER(TYPE_NAME) = 'PEGWINDOW';
   select count(*) into PegCount from TBLSTGTESTCASEVALID where FEEDPROCESSID = FEEDPROCESSID1 and Keyword = 'PegWindow';
   if ( PegCount >= 1 )
   then
   begin
   select  min(ID) into FirstPegWindowId from TBLSTGTESTCASEVALID where FEEDPROCESSID = FEEDPROCESSID1 and Keyword = 'PegWindow'  ;
   end;
   end if;



   insert into TBLSTGTESTCASEVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
   select valid.ID,'Object ['||valid.OBJECT ||'] does not exist. ','0',FEEDPROCESSID from TBLSTGTESTCASEVALID valid
   left join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT  
   where valid.FEEDPROCESSID = FEEDPROCESSID1 and  ob.OBJECT_NAME_ID is null and valid.OBJECT is not null order by ID;

   insert into TBLSTGTESTCASEVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
   select valid.ID,'Object ['||valid.OBJECT||'] for Application  ['||ra.APP_SHORT_NAME||'] does not exist. ','0',FEEDPROCESSID from TBLSTGTESTCASEVALID valid
   --join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT    
   --left join T_REGISTED_OBJECT ro on ob.OBJECT_NAME_ID = ro.OBJECT_NAME_ID and  ro.APPLICATION_ID in (
   --select rr.APPLICATION_ID from REL_APP_TESTCASE rr where rr.TEST_CASE_ID = valid.TESTCASEID
   --)
   --left join  T_REGISTERED_APPS ra on ro.APPLICATION_ID = ra.APPLICATION_ID
   join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT  
   join REL_APP_TESTCASE rel on rel.TEST_CASE_ID = valid.TESTCASEID
   left join T_REGISTED_OBJECT ro on ob.OBJECT_NAME_ID = ro.OBJECT_NAME_ID and  ro.APPLICATION_ID = rel.APPLICATION_ID 
   left join  T_REGISTERED_APPS ra on rel.APPLICATION_ID = ra.APPLICATION_ID
   where valid.FEEDPROCESSID = FEEDPROCESSID1 and valid.OBJECT is not null and ro.OBJECT_ID is null order by ID;



    insert into TBLSTGTESTCASEVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
   select valid.ID,'Keyword ['|| valid.KEYWORD ||'] does not exist. ','0',FEEDPROCESSID from TBLSTGTESTCASEVALID valid
   left join  T_KEYWORD ky on ky.KEY_WORD_NAME = valid.KEYWORD
   where valid.FEEDPROCESSID = FEEDPROCESSID1 and ky.KEY_WORD_ID is null order by ID;

   if(FirstPegWindowId != -1)
   then
   begin
   insert into TBLSTGTESTCASEVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
   select distinct valid.ID,' Keyword ['||valid.KEYWORD||'] does not exist. ','0',FEEDPROCESSID from TBLSTGTESTCASEVALID valid   
   join T_KEYWORD ky on ky.KEY_WORD_NAME = valid.KEYWORD
   Where valid.FEEDPROCESSID = FEEDPROCESSID1 and lower(valid.KEYWORD) not in ('pegwindow','dbcompare','copyexcelrangetoclipboard',
   'executecommand','killapplication','loop','resumenext','startapplication','waitforseconds') and valid.ID <=
   FirstPegWindowId order by ID;
   end;
   else
   begin
   insert into TBLSTGTESTCASEVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
   select distinct valid.ID,' Keyword ['||valid.KEYWORD||'] does not exist. ','0',FEEDPROCESSID from TBLSTGTESTCASEVALID valid   
   join T_KEYWORD ky on ky.KEY_WORD_NAME = valid.KEYWORD
   Where valid.FEEDPROCESSID = FEEDPROCESSID1 and lower(valid.KEYWORD) not in ('pegwindow','dbcompare','copyexcelrangetoclipboard',
   'executecommand','killapplication','loop','resumenext','startapplication','waitforseconds') order by ID;
   end;
   end if;

   insert into TBLSTGTESTCASEVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
   select distinct valid.ID,'Keyword ['||valid.KEYWORD||'] can not be applied to Object ['||valid.OBJECT||']. ','0',FEEDPROCESSID from TBLSTGTESTCASEVALID valid 
    -- join REL_APP_TESTCASE rel on rel.TEST_CASE_ID = valid.TESTCASEID
   join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT 
   join T_KEYWORD ky on ky.KEY_WORD_NAME = valid.KEYWORD
   join  REL_APP_TESTCASE rat on rat.test_case_id = valid.testcaseid
   left join   T_DIC_RELATION_KEYWORD dic on ky.KEY_WORD_ID = dic.KEY_WORD_ID
      and dic.TYPE_ID in ( select ro.TYPE_ID from T_REGISTED_OBJECT ro
                          join REL_APP_TESTCASE rel on rel.APPLICATION_ID = ro.APPLICATION_ID and rel.APPLICATION_ID = rat.APPLICATION_ID
                          where ob.OBJECT_NAME_ID = ro.OBJECT_NAME_ID 
                           )
   where valid.FEEDPROCESSID = FEEDPROCESSID1 and valid.OBJECT is not null and dic.TYPE_ID is null order by ID;


   DECLARE
  row_id number:=0;
  totalRow NUMBER:=0;
  keyword varchar2(200);
  PegObjectType varchar2(200);
  PegObjectTypeRowId number;
  PegObjectTypeRowName varchar2(500):='';
  ChildObjectType varchar2(200);
  lObject varchar2(200);
BEGIN
  select count(*) into totalRow  from TBLSTGTESTCASEVALID where FEEDPROCESSID = FEEDPROCESSID1;

  DBMS_OUTPUT.put_line('simple comment');
  DBMS_OUTPUT.put_line(totalRow);
    DBMS_OUTPUT.put_line(row_id);

  WHILE totalRow > row_id
  LOOP
  select valid.OBJECT,valid.KEYWORD into lObject,keyword from TBLSTGTESTCASEVALID valid where valid.FEEDPROCESSID = FEEDPROCESSID1 
  and valid.id= row_id order by ID;

  --DBMS_OUTPUT.PUT_LINE( 'test : ' || row_id || ' keyword : ' || keyword);

    if (lObject is not null)
    then 
    begin

    if (upper(keyword)='PEGWINDOW' )
    then
    begin
    PegObjectTypeRowId := row_id;
       PegObjectTypeRowName := lObject;

    end;
    else
    begin

    if (PegObjectTypeRowName = '')
    then 
    begin 

    declare lMatchcount Number:=0;
    begin

      select count(*) into lMatchcount from 
      TBLSTGTESTCASEVALID valid
       join REL_APP_TESTCASE rel on rel.TEST_CASE_ID = valid.TESTCASEID
      join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT 
      left join T_REGISTED_OBJECT ro on ob.OBJECT_NAME_ID = ro.OBJECT_NAME_ID 
                          and rel.APPLICATION_ID = ro.APPLICATION_ID
                          and ro.OBJECT_TYPE in (select  distinct ro.OBJECT_TYPE 
      from TBLSTGTESTCASEVALID valid  
      join REL_APP_TESTCASE rel on rel.TEST_CASE_ID = valid.TESTCASEID
      join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT 
      join T_REGISTED_OBJECT ro on ob.OBJECT_NAME_ID = ro.OBJECT_NAME_ID 
                          and rel.APPLICATION_ID = ro.APPLICATION_ID
      where valid.FEEDPROCESSID = FEEDPROCESSID1 and valid.OBJECT is not null and valid.ID= PegObjectTypeRowId)
    where valid.FEEDPROCESSID = FEEDPROCESSID1 and valid.OBJECT is not null and valid.ID= row_id  
    ;

    if(lMatchcount = 0)
    then 
    begin

    insert into TBLSTGTESTCASEVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
   select distinct valid.ID,'Object ['||valid.OBJECT||'] does not belong to Pegwindow. ','0',FEEDPROCESSID
         from TBLSTGTESTCASEVALID valid
       join REL_APP_TESTCASE rel on rel.TEST_CASE_ID = valid.TESTCASEID
      join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT 
      left join T_REGISTED_OBJECT ro on ob.OBJECT_NAME_ID = ro.OBJECT_NAME_ID 
                          and rel.APPLICATION_ID = ro.APPLICATION_ID
                          and ro.OBJECT_TYPE in (select  distinct ro.OBJECT_TYPE 
      from TBLSTGTESTCASEVALID valid  
      join REL_APP_TESTCASE rel on rel.TEST_CASE_ID = valid.TESTCASEID
      join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT 
      join T_REGISTED_OBJECT ro on ob.OBJECT_NAME_ID = ro.OBJECT_NAME_ID 
                          and rel.APPLICATION_ID = ro.APPLICATION_ID
      where valid.FEEDPROCESSID = FEEDPROCESSID1 and valid.OBJECT is not null and valid.ID= PegObjectTypeRowId)
    where valid.FEEDPROCESSID = FEEDPROCESSID1 and valid.OBJECT is not null and valid.ID= row_id and ro.OBJECT_NAME_ID is null 
    order by ID;


    end;
    end if;



    end;



    end;
    else
    begin

    declare MatchCount number:= 0;
    begin

    select count(*) into MatchCount
         from TBLSTGTESTCASEVALID valid
       join REL_APP_TESTCASE rel on rel.TEST_CASE_ID = valid.TESTCASEID
      join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT 
      join T_REGISTED_OBJECT ro on ob.OBJECT_NAME_ID = ro.OBJECT_NAME_ID 
                          and rel.APPLICATION_ID = ro.APPLICATION_ID
                          and ro.OBJECT_TYPE in (select  distinct ro.OBJECT_TYPE 
      from TBLSTGTESTCASEVALID valid  
      join REL_APP_TESTCASE rel on rel.TEST_CASE_ID = valid.TESTCASEID
      join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT 
      join T_REGISTED_OBJECT ro on ob.OBJECT_NAME_ID = ro.OBJECT_NAME_ID 
                          and rel.APPLICATION_ID = ro.APPLICATION_ID
      where valid.FEEDPROCESSID = FEEDPROCESSID1 and valid.OBJECT is not null and valid.ID= PegObjectTypeRowId)
    where valid.FEEDPROCESSID = FEEDPROCESSID1 and valid.OBJECT is not null and valid.ID= row_id  
    ; 

    if (MatchCount = 0)
    then 
    begin
    insert into TBLSTGTESTCASEVALIDATION(ID,VALIDATIONMSG,ISVALID,FEEDPROCESSID)
    select distinct valid.ID,'Object ['||valid.OBJECT||'] does not belong to Pegwindow ['||PegObjectTypeRowName||']. ','0',FEEDPROCESSID
         from TBLSTGTESTCASEVALID valid
       join REL_APP_TESTCASE rel on rel.TEST_CASE_ID = valid.TESTCASEID
      join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT 
      left join T_REGISTED_OBJECT ro on ob.OBJECT_NAME_ID = ro.OBJECT_NAME_ID 
                          and rel.APPLICATION_ID = ro.APPLICATION_ID
                          and ro.OBJECT_TYPE in (select  distinct ro.OBJECT_TYPE 
      from TBLSTGTESTCASEVALID valid  
      join REL_APP_TESTCASE rel on rel.TEST_CASE_ID = valid.TESTCASEID
      join  T_OBJECT_NAMEINFO ob on ob.OBJECT_HAPPY_NAME = valid.OBJECT 
      join T_REGISTED_OBJECT ro on ob.OBJECT_NAME_ID = ro.OBJECT_NAME_ID 
                          and rel.APPLICATION_ID = ro.APPLICATION_ID
      where valid.FEEDPROCESSID = FEEDPROCESSID1 and valid.OBJECT is not null and valid.ID= PegObjectTypeRowId)
    where valid.FEEDPROCESSID = FEEDPROCESSID1 and valid.OBJECT is not null and valid.ID= row_id and ro.OBJECT_NAME_ID is null 
    order by ID;
    end;
    end if;

    end;

    end;
    end if;






   -- dbms_output.put_line('counter' || row_id );
    end;
    end if;

    end;

    end if;
    row_id:= row_id + 1;
  END LOOP;
END;


end  ;  

end "SP_CheckValidationTestCase";



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
                                     ERROR VARCHAR2(500);
							BEGIN
                              -----------test suite not exist start-------------
                                    SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=54;
                                    SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=54;

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
                                                      ERRORDESCRIPTION,
                                                      FEEDPROCESSINGDETAILSID,
                                                      FEEDPROCESSID,
                                                      TESTSUITE
                                                  )
                                                  SELECT DISTINCT
                                                      (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                      VALIDATETYPE,
                                                      'Validation',
                                                      '',
                                                      VALIDATETYPE,
                                                      ERROR,
                                                      tbl.APPLICATION,
                                                      tbl.TESTCASENAME,
                                                      ERROR,
                                                      I_INDEX.FEEDPROCESSDETAILID,
                                                      FEEDPROCESSID1,
                                                      tbl.testsuitename
                                                  FROM TBLSTGTESTCASE tbl 
                                                  join T_TEST_CASE_SUMMARY tc ON UPPER(tc.TEST_CASE_NAME)=UPPER(tbl.TESTCASENAME)
                                                  left join T_test_suite ts on UPPER(ts.test_suite_name)=UPPER(tbl.testsuitename)
                                                  left join rel_test_case_test_suite  rtc on rtc.TEST_CASE_ID = tc.TEST_CASE_ID and ts.test_suite_id = rtc.test_suite_id
                                                  where tbl.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID and ts.test_suite_id is null;

                                    END;
                             -----------test suite not exist end-------------
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
                                                    INNER JOIN TEMPFALSEAPPLICATION ON Upper(TEMPFALSEAPPLICATION.TESTSUITNAME) = Upper(tblstgtestcase.testsuitename)
                                                        AND Upper(TEMPFALSEAPPLICATION.TESTCASENAME) = Upper(tblstgtestcase.testcasename)
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
                                            --Foram Start
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
                                                TESTSUITE,
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
                                              '',
                                              TESTSUITENAME,
                                              DATASETNAME,
                                              '',
                                              '',
                                              '',
                                              ERROR,
                                              '',
                                              I_INDEX.FEEDPROCESSDETAILID,
                                              FEEDPROCESSID1
                                            FROM (
                                            SELECT NVL(OBJECT, 'N/A'), APPLICATION,KEYWORD,DATASETNAME,TESTSUITENAME, ROWNUMBER,COUNT(1)
                                              FROM TBLSTGTESTCASE
                                              WHERE FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                              GROUP BY NVL(OBJECT, 'N/A'),TESTSUITENAME, KEYWORD,DATASETNAME,ROWNUMBER, APPLICATION
                                              HAVING COUNT(1) > 1
                                              ORDER BY testsuitename,ROWNUMBER
                                            )xyz;


                                            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=53;
                                            SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=53;

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
                                          select tblstgtestcase.datasetname as DATASETNAME,
                                                    tblstgtestcase.application as APPLICATION,
                                                    tblstgtestcase.testcasename as TESTCASENAME,
                                                    count(1) from T_TEST_DATA_SUMMARY
                                            inner join tblstgtestcase on T_TEST_DATA_SUMMARY.ALIAS_NAME=tblstgtestcase.DATASETNAME
                                            inner join t_test_case_summary on Upper(t_test_case_summary.TEST_CASE_NAME) = Upper(tblstgtestcase.TESTCASENAME)
                                            inner join REL_TC_DATA_SUMMARY on REL_TC_DATA_SUMMARY.TEST_CASE_ID != t_test_case_summary.TEST_CASE_ID
                                                                          and T_TEST_DATA_SUMMARY.DATA_SUMMARY_ID =REL_TC_DATA_SUMMARY.DATA_SUMMARY_ID
                                            where tblstgtestcase.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID --and T_TEST_DATA_SUMMARY.DATA_SUMMARY_ID =REL_TC_DATA_SUMMARY.DATA_SUMMARY_ID
                                            --and REL_TC_DATA_SUMMARY.TEST_CASE_ID != t_test_case_summary.TEST_CASE_ID
                                            group by tblstgtestcase.datasetname,tblstgtestcase.application,tblstgtestcase.testcasename
                                             having count(1)>1 order by tblstgtestcase.TESTCASENAME)xyz;


                                             SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=53;
                                            SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=53;
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
                                            FROM (  select tblstgtestcase.datasetname as DATASETNAME,
                                                    tblstgtestcase.application as APPLICATION,
                                                    tblstgtestcase.testcasename as TESTCASENAME,
                                                    count(1) from T_TEST_DATA_SUMMARY
                                            inner join tblstgtestcase on Upper(T_TEST_DATA_SUMMARY.ALIAS_NAME)=Upper(tblstgtestcase.DATASETNAME)
                                            where tblstgtestcase.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID and  Upper(tblstgtestcase.TESTCASENAME) not in(select Upper(test_case_name) from t_test_case_summary)
                                            group by tblstgtestcase.datasetname,tblstgtestcase.application,tblstgtestcase.testcasename
                                             having count(1)>1 order by tblstgtestcase.TESTCASENAME)xyz;

                                              SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=54;
                                            SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=54;

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
                                                TESTSUITE,
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
                                              TESTSUITENAME,
                                              '',
                                              '',
                                              '',
                                              '',
                                              ERROR,
                                              '',
                                              I_INDEX.FEEDPROCESSDETAILID,
                                              FEEDPROCESSID1
                                            FROM (  
                                            select stg.APPLICATION as APPLICATION,
                                                    stg.testsuitename as TESTSUITENAME, 
                                                    stg.testcasename as TESTCASENAME,
                                                    count(1) from t_test_case_summary tc
                                            inner join tblstgtestcase stg on upper(tc.test_case_name)=upper(stg.TESTCASENAME)
                                            inner join t_test_suite ts on upper(ts.TEST_SUITE_NAME)=upper(stg.TESTSUITENAME)
                                            inner join REL_TEST_CASE_TEST_SUITE reltc on tc.TEST_CASE_ID = reltc.TEST_CASE_ID
                                                                                  and ts.TEST_SUITE_ID!=reltc.TEST_SUITE_ID
                                            where stg.FEEDPROCESSDETAILID=I_INDEX.FEEDPROCESSDETAILID
                                             group by stg.testcasename,stg.testsuitename,stg.APPLICATION
                                              having count(1)>1
                                          )xyz;

                                            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=51;
                                          SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=51;
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
                                                TESTSUITE,
                                                DATASETNAME,
                                                TESTSTEPNUMBER,
                                                OBJECTNAME,
                                                COMMENTDATA,
                                                ERRORDESCRIPTION,
                                                PROGRAMLOCATION,
                                                FEEDPROCESSINGDETAILSID,
                                                FEEDPROCESSID
                                            )
                                            select DISTINCT
                                              SYSDATE,
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
                                              '',
                                              ERROR,
                                              '',
                                              I_INDEX.FEEDPROCESSDETAILID,
                                              FEEDPROCESSID1
                                              from
                                              ( select application from (
                                                select Rownum as ID,application from (
                                                select application as application from 
                                                tblstgtestcase
                                                where FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID --and application in (regexp_substr(application, '[^,]+', 1, 1))
                                                group by application )) where ID > 1  
                                              ) 
                                              group by application;

                                             --Foram End
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

                                                            AND NOT EXISTS (
                                                                SELECT 1
                                                                FROM TEMPFALSEAPPLICATION
                                                                WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                    AND tempfalseapplication.FLAG = 0
                                                                    AND ROWNUM=1
                                                            )

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

                                                            AND NOT EXISTS (
                                                                SELECT 1
                                                                FROM TEMPFALSEAPPLICATION
                                                                WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                    AND tempfalseapplication.FLAG = 0
                                                                    AND ROWNUM=1
                                                            )

                                                        GROUP BY tblstgtestcase.TESTCASENAME, tblstgtestcase.DATASETNAME,tblstgtestcase.Application
                                                        ORDER BY 1,3
                                                    ) y
                                                    ON x.TESTCASENAME = y.TESTCASENAME
                                                        AND x.DATASETNAME = y.DATASETNAME
                                                        AND x.CountofSteps <> y.CountofSteps    
                                                ) xyz;   
                                            -- Insert log for count of steps in spreadsheet and count of steps in warehouse does not match - start
                                            --cherish Patel
                                                --SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=7;
                                                --SELECT MESSAGESYMBOL INTO VALIDATETYPE FROM TBLMESSAGE WHERE ID=7;

                                                --INSERT INTO "TBLLOGREPORT"
                                                --(
                                                --  CREATIONDATE,
                                                --  MESSAGETYPE,
                                                --  ACTION,
                                                --  CELLADDRESS,
                                                --  VALIDATIONNAME,
                                                 -- VALIDATIONDESCRIPTION,
                                                  --APPLICATIONNAME,
                                                  --TESTCASENAME,
                                                  --DATASETNAME,
                                                  --TESTSTEPNUMBER,
                                                  --OBJECTNAME,
                                                  --COMMENTDATA,
                                                  --ERRORDESCRIPTION,
                                                  --PROGRAMLOCATION,
                                                  --FEEDPROCESSINGDETAILSID,
                                                  --FEEDPROCESSID
                                                --)
                                                --SELECT DISTINCT
                                                  --  (SELECT SYSDATE FROM DUAL) AS CREATIONDATE,
                                                  --  VALIDATETYPE,
                                                  --  'Validation',
                                                   -- '',
                                                    --VALIDATETYPE,
                                                   -- ERROR,
                                                    --APPLICATION,
                                                    --TESTCASENAME,
                                                    --'',
                                                    --'',
                                                    --'',
                                                    --'',
                                                    --ERROR,
                                                    --'',
                                                    --I_INDEX.FEEDPROCESSDETAILID,
                                                    --FEEDPROCESSID1
                                                --FROM (
                                                  --  SELECT DISTINCT 
                                                  --      X.TESTCASENAME,
                                                  --      X.DATASETNAME,
                                                   --     X.APPLICATION
                                                   -- FROM (
                                                   --     SELECT TESTCASENAME,DATASETNAME,MAX(ROWNUMBER) as CountofSteps,APPLICATION
                                                   --     FROM TBLSTGTESTCASE
                                                   --     WHERE KEYWORD IS NOT NULL
                                                   --         AND FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                   --         AND DATASETMODE IS NULL
                                                            /*
                                                            AND NOT EXISTS (
                                                                SELECT 1
                                                                FROM TEMPFALSEAPPLICATION
                                                                WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                    AND tempfalseapplication.FLAG = 0
                                                                    AND ROWNUM=1
                                                            )
                                                            */
                                                  --      GROUP BY TESTCASENAME,DATASETNAME,APPLICATION
                                                  --      ORDER BY 1,3
                                                  --  ) x    
                                                  --  INNER JOIN 
                                                  --  (
                                                  --      SELECT tblstgtestcase.TESTCASENAME, tblstgtestcase.DATASETNAME, MAX(RUN_ORDER) as CountofSteps, tblstgtestcase.Application
                                                  --      FROM tblstgtestcase
                                                  --      INNER JOIN t_test_case_summary ON UPPER(t_test_case_summary.test_case_name) = UPPER(tblstgtestcase.TESTCASENAME)
                                                   --     INNER JOIN t_test_data_summary ON UPPER(t_test_data_summary.alias_name) = UPPER(tblstgtestcase.DATASETNAME)
                                                    --    INNER JOIN T_KEYWORD ON UPPER(T_KEYWORD.KEY_WORD_NAME) = UPPER(tblstgtestcase.KEYWORD)
                                                     --   INNER JOIN T_TEST_STEPS ON T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.TEST_CASE_ID
                                                     --       AND T_TEST_STEPS.KEY_WORD_ID = T_KEYWORD.KEY_WORD_ID
                                                     --       AND T_TEST_STEPS.RUN_ORDER = tblstgtestcase.ROWNUMBER
                                                     --       AND T_TEST_STEPS.TEST_CASE_ID = t_test_case_summary.test_case_id
                                                     --   WHERE tblstgtestcase.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                        /*
                                                            AND NOT EXISTS (
                                                                SELECT 1
                                                                FROM TEMPFALSEAPPLICATION
                                                                WHERE tempfalseapplication.testcasename = tblstgtestcase.testcasename
                                                                    AND tempfalseapplication.FLAG = 0
                                                                    AND ROWNUM=1
                                                            )
                                                            */
                                                      --  GROUP BY tblstgtestcase.TESTCASENAME, tblstgtestcase.DATASETNAME,tblstgtestcase.Application
                                                      --  ORDER BY 1,3
                                                    --) y
                                                   -- ON x.TESTCASENAME = y.TESTCASENAME
                                                   --     AND x.DATASETNAME = y.DATASETNAME
                                                   --     AND x.CountofSteps <> y.CountofSteps    
                                                ---) xyz;        
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
                                                 --INNER JOIN T_REGISTED_OBJECT treg ON treg.OBJECT_NAME_ID = tobjn.OBJECT_NAME_ID
                                                 INNER JOIN TEMPFALSEAPPLICATION f ON f.TESTCASENAME = stg.TESTCASENAME
                                                 INNER JOIN T_REGISTERED_APPS aa on upper(aa.app_short_name) = upper(f.application)
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
                                                   INNER JOIN T_REGISTED_OBJECT treg ON tdic.TYPE_ID = treg.TYPE_ID and treg.APPLICATION_ID = aa.APPLICATION_ID
                                                  WHERE tk.KEY_WORD_NAME = stg.KEYWORD
                                                  AND treg.OBJECT_NAME_ID = tobjn.OBJECT_NAME_ID
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


       -- insert into Storyboard_Temp (Name,Type,StoryboardName)        
       -- select distinct regexp_substr(applicationname,'[^,]+', 1, level), 'APPLICATION',StoryboardName from tblstgstoryboard  where feedprocessdetailid = I_INDEX.FEEDPROCESSDETAILID
       -- connect by regexp_substr(applicationname, '[^,]+', 1, level) is not null; 

 insert into Storyboard_Temp (Name,Type,StoryboardName)        


         SELECT Application,'APPLICATION',StoryboardName  FROM (
                                                      SELECT 
                                                        trim(regexp_substr(applicationname, '[^,]+', 1, level)) Application,StoryboardName

                                                    FROM (
                                                        SELECT DISTINCT applicationname ,StoryboardName
                                                        FROM tblstgstoryboard 
                                                        WHERE tblstgstoryboard.FEEDPROCESSDETAILID = I_INDEX.FEEDPROCESSDETAILID
                                                    ) t
                                                    CONNECT BY instr(applicationname, ',', 1, level - 1) > 0
                                            ) xy

                                            WHERE EXISTS (
                                                SELECT 1
                                                FROM T_REGISTERED_APPS 
                                                WHERE T_REGISTERED_APPS.APP_SHORT_NAME = xy.Application
                                            );


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


create or replace PROCEDURE  "SP_SavaAs_TestCase_One_Dataset" (
    OldTestcaseId IN NUMBER,
    NewTestcaseName IN VARCHAR2,
    NewTestcaseDes IN VARCHAR2,
    TestsuiteId IN NUMBER,
    ProjectId IN NUMBER,
    OldDatasetName IN VARCHAR2,
    Suffix IN VARCHAR2,
    LoginUser IN VARCHAR2,
    RESULT OUT varchar2
)
IS
BEGIN
DECLARE
        DupTestCase number:=0;
        DupDataset number:=0;
        NewTestcaseId number:=0;
        NewDatasetId number:=0;
        CURRENTDATE TIMESTAMP;
        BEGIN
          -- check duplicate testcasename exist or not
        select count(*) into DupTestCase from T_TEST_CASE_SUMMARY where lower(TEST_CASE_NAME) = lower(NewTestcaseName);

 

         if (DupTestCase > 0 )
          then
          begin
             RESULT:= 'This Testcase already exist';
          end;
        end if;

 

        -- check duplicate datasetname exist or not
         select count(*) into DupDataset from T_TEST_DATA_SUMMARY 
         where ALIAS_NAME = CONCAT(OldDatasetName,Suffix);
         if (DupDataset > 0 )
          then
          begin
             RESULT:= 'This dataset already exist';
          end;
        end if;

 

        if(DupTestCase = 0 and DupDataset = 0)
        then
        begin

 

        SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

 

        SELECT T_TEST_CASE_SUMMARY_SEQ.nextval INTO NewTestcaseId FROM DUAL;
        insert into T_TEST_CASE_SUMMARY (TEST_CASE_ID,TEST_CASE_NAME, TEST_STEP_DESCRIPTION,TEST_STEP_CREATE_TIME,TEST_STEP_CREATOR )
        select NewTestcaseId,NewTestcaseName,NewTestcaseDes,CURRENTDATE,LoginUser from dual;

 

        insert into REL_TEST_CASE_TEST_SUITE(RELATIONSHIP_ID,TEST_SUITE_ID,TEST_CASE_ID)
        select REL_TEST_CASE_TEST_SUITE_SEQ.nextval,TestsuiteId,NewTestcaseId from dual;

 

        insert into REL_APP_TESTCASE(RELATIONSHIP_ID,APPLICATION_ID,TEST_CASE_ID)
        select  REL_APP_TESTCASE_SEQ.nextval,APPLICATION_ID,NewTestcaseId from REL_APP_TESTCASE where TEST_CASE_ID = OldTestcaseId;

 

        SELECT T_TEST_STEPS_SEQ.nextval INTO NewDatasetId FROM DUAL;
        insert into T_TEST_DATA_SUMMARY(DATA_SUMMARY_ID,ALIAS_NAME,SHARE_MARK,STATUS,CREATE_TIME)
        VALUES(NewDatasetId, CONCAT(OldDatasetName,Suffix),1,0,CURRENTDATE);

 

        insert into REL_TC_DATA_SUMMARY(ID,DATA_SUMMARY_ID,TEST_CASE_ID,CREATE_TIME)
        select T_TEST_STEPS_SEQ.nextval,NewDatasetId,NewTestcaseId,CURRENTDATE from dual;

 

        insert into T_TEST_STEPS(STEPS_ID,RUN_ORDER,TEST_CASE_ID,COLUMN_ROW_SETTING,"COMMENT",IS_RUNNABLE,
        KEY_WORD_ID,OBJECT_ID,OBJECT_NAME_ID,VALUE_SETTING)
        select T_TEST_STEPS_SEQ.nextval,RUN_ORDER,NewTestcaseId,COLUMN_ROW_SETTING,"COMMENT",IS_RUNNABLE,
        KEY_WORD_ID,OBJECT_ID,OBJECT_NAME_ID,VALUE_SETTING  from T_TEST_STEPS where TEST_CASE_ID = OldTestcaseId;

 

        insert into TEST_DATA_SETTING (DATA_SETTING_ID,STEPS_ID,DATA_SUMMARY_ID,LOOP_ID,DATA_VALUE,POOL_ID,VALUE_OR_OBJECT,
        VERSION,DATA_DIRECTION,CREATE_TIME )
        select  T_TEST_STEPS_SEQ.nextval,newsetting.STEPS_ID,newsetting.DATA_SUMMARY_ID,setting.LOOP_ID,setting.DATA_VALUE,setting.POOL_ID,setting.VALUE_OR_OBJECT,
           setting.VERSION,setting.DATA_DIRECTION,CURRENTDATE
        from TEST_DATA_SETTING setting
        join T_TEST_STEPS step on setting.STEPS_ID = step.STEPS_ID
        join REL_TC_DATA_SUMMARY dataset on step.TEST_CASE_ID= dataset.TEST_CASE_ID and setting.DATA_SUMMARY_ID = dataset.DATA_SUMMARY_ID
        join T_TEST_DATA_SUMMARY ds on dataset.DATA_SUMMARY_ID = ds.DATA_SUMMARY_ID
        join (
              select nds.ALIAS_NAME,nstep.RUN_ORDER,nstep.STEPS_ID,ndataset.DATA_SUMMARY_ID
              from T_TEST_STEPS nstep 
              join REL_TC_DATA_SUMMARY ndataset on nstep.TEST_CASE_ID= ndataset.TEST_CASE_ID 
              join T_TEST_DATA_SUMMARY nds on ndataset.DATA_SUMMARY_ID = nds.DATA_SUMMARY_ID where nstep.TEST_CASE_ID = NewTestcaseId
              )
              newsetting on CONCAT(ds.ALIAS_NAME,Suffix) = newsetting.ALIAS_NAME and step.RUN_ORDER = newsetting.RUN_ORDER
        where step.TEST_CASE_ID = OldTestcaseId; 
        
         EXECUTE IMMEDIATE 'ALTER MATERIALIZED VIEW MV_LAST_TC_INFO COMPILE';
         RESULT:= 'success';
        end;
        end if;

 

         select RESULT into RESULT from dual;

 

        end  ;  

 

end "SP_SavaAs_TestCase_One_Dataset";

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
  SELECT DISTINCT stg.STEPSID,t.OBJECT_ID, tk.KEY_WORD_ID,t.OBJECT_NAME_ID, stg.PARAMETER, stg.LCOMMENTS
  FROM TBLSTGTESTCASESAVE stg
  INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD)
  CROSS JOIN TABLE(GETTOPOBJECT(stg.OBJECT, ApplicationId)) t
    WHERE stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID     
      and stg.KEYWORD IS NOT NULL 
      and stg.Type= 'Update'  and stg.WHICHTABLE is null  ) TMP
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
   
      

 


UPDATE TEST_DATA_SETTING td SET (td.DATA_VALUE,td.DATA_DIRECTION) = (select distinct stg.DATASETVALUE,stg.SKIP from TBLSTGTESTCASESAVE stg 
                                  where stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.Type= 'Update'  and stg.WHICHTABLE is null 
                                  and stg.DATASETID > 0 and stg.DATA_SETTING_ID > 0
                                  and td.DATA_SETTING_ID= stg.DATA_SETTING_ID)
where 
EXISTS (select distinct stg.DATASETVALUE,stg.SKIP from TBLSTGTESTCASESAVE stg 
                                  where stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.Type= 'Update'  and stg.WHICHTABLE is null 
                                  and stg.DATASETID > 0 and stg.DATA_SETTING_ID > 0
                                  and td.DATA_SETTING_ID= stg.DATA_SETTING_ID);

 

 


INSERT INTO T_TEST_STEPS(STEPS_ID,RUN_ORDER,KEY_WORD_ID,TEST_CASE_ID,OBJECT_ID,COLUMN_ROW_SETTING,"COMMENT",OBJECT_NAME_ID)
select  T_TEST_STEPS_SEQ.nextval,ROWNUMBER,KEY_WORD_ID,TESTCASEID,object_id,PARAMETER,LCOMMENTS,object_name_id from
(SELECT distinct stg.ROWNUMBER,tk.KEY_WORD_ID,stg.TESTCASEID,t.object_id,stg.PARAMETER,stg.LCOMMENTS,t.object_name_id
from TBLSTGTESTCASESAVE stg 
INNER JOIN T_KEYWORD tk ON UPPER(tk.KEY_WORD_NAME) = UPPER(stg.KEYWORD) 
CROSS JOIN TABLE(GETTOPOBJECT(stg.OBJECT, ApplicationId)) t
where stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID and stg.STEPSID <= 0
AND NOT EXISTS (
                                                    SELECT 1
                                                    FROM T_TEST_STEPS
                                                    WHERE T_TEST_STEPS.KEY_WORD_ID = tk.key_word_id
                                                        AND T_TEST_STEPS.RUN_ORDER = stg.rownumber
                                                        AND T_TEST_STEPS.TEST_CASE_ID = stg.testcaseid
                                                        AND ROWNUM=1
                                                )
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
  WHERE stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID
    and stg.KEYWORD IS NOT NULL  and stg.STEPSID <= 0
    AND NOT EXISTS (
       SELECT 1
       FROM TEST_DATA_SETTING
       WHERE test_data_setting.steps_id = t_test_steps.steps_id
        AND TEST_DATA_SETTING.DATA_SUMMARY_ID = stg.datasetid
        AND ROWNUM=1
    )
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
  WHERE stg.FEEDPROCESSDETAILID = FEEDPROCESSDETAIL_ID
    and stg.KEYWORD IS NOT NULL  and stg.STEPSID > 0
    AND NOT EXISTS (
       SELECT 1
       FROM TEST_DATA_SETTING
       WHERE test_data_setting.steps_id = t_test_steps.steps_id
        AND TEST_DATA_SETTING.DATA_SUMMARY_ID = stg.datasetid
        AND ROWNUM=1
    )
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

CREATE OR REPLACE PROCEDURE SP_LOG(
procname IN VARCHAR2,
msgtype IN VARCHAR2,
message IN VARCHAR2,
IDS IN VARCHAR2
)
IS BEGIN
INSERT INTO TBL_PROCEDURE_LOG(
  LOGID,
  PROC_NAME,
  MSG_TYPE,
  MESSAGE,
  LOG_TIMESTAMP,
  PARAMETER
)VALUES(
 procedure_log_seq.nextval,
      procname,
      msgtype,
      message,
      CURRENT_TIMESTAMP,
      IDS);
EXCEPTION
WHEN OTHERS THEN
BEGIN
  DBMS_OUTPUT.PUT_LINE($$PLSQL_UNIT ||'-' || SQLCODE || '-' ||SQLERRM);
END;
END SP_LOG;

/

create or replace PROCEDURE "DeleteResultSets" (
HistId IN number
)
IS
DelCount number;
Begin

 


select count(*) into DelCount from  t_test_report_steps where TEST_REPORT_ID in (select tr.TEST_REPORT_ID from t_test_report tr where tr.HIST_ID= HistId);
insert into deletelog select  SEQ_DELETELOG.nextval,'T_TEST_REPORT_STEPS','DELETE T_TEST_REPORT_STEPS, ' || DelCount,HistId,
  'HistId',SYSTIMESTAMP from dual;
delete t_test_report_steps where TEST_REPORT_ID in (select tr.TEST_REPORT_ID from t_test_report tr where tr.HIST_ID= HistId);

 

select count(*) into DelCount from  t_test_report where HIST_ID= HistId;
insert into deletelog select  SEQ_DELETELOG.nextval,'T_TEST_REPORT','DELETE T_TEST_REPORT, ' || DelCount,HistId,
  'HistId',SYSTIMESTAMP from dual;
delete t_test_report where HIST_ID= HistId;

 

select count(*) into DelCount from  t_proj_test_result where HIST_ID= HistId;
insert into deletelog select  SEQ_DELETELOG.nextval,'T_PROJ_TEST_RESULT','DELETE T_PROJ_TEST_RESULT, ' || DelCount,HistId,
  'HistId',SYSTIMESTAMP from dual;
delete t_proj_test_result where HIST_ID= HistId;

 

End;

/

create or replace PROCEDURE  SP_SAVEAS_RESULTSET (
  OldCompare_HISTID in NUMBER,
  OldBaseline_HISTID in NUMBER
)
IS
BEGIN
DECLARE
        NewCompare_HISTID number:=0;
        NewBaseline_HISTID number:=0;
        NewCompare_REPORTID number:=0;
        NewBaseline_REPORTID number:=0;
        OldCompare_REPORTID number:=0;
        OldBaseline_REPORTID number:=0;
        NewComp_LATEST_TEST_MARK_ID number:=0;
        NewBase_LATEST_TEST_MARK_ID number:=0;
        BEGIN
        
        IF OldCompare_HISTID != 0 THEN
            BEGIN
                SELECT SEQ_TESTRESULT_ID.nextval INTO NewCompare_HISTID FROM DUAL;
                SELECT SEQ_TESTRESULT_ID.nextval INTO NewComp_LATEST_TEST_MARK_ID FROM DUAL;
		
		        insert into t_proj_test_result (HIST_ID,TEST_CASE_ID, STORYBOARD_DETAIL_ID,TEST_BEGIN_TIME,TEST_END_TIME,TEST_RESULT_IN_TEXT,TEST_MODE,TEST_RESULT,TESTER_ID,CREATE_TIME,LATEST_TEST_MARK_ID,RELY_TEST_CASE_ID,RESULT_ALIAS_NAME,RESULT_DESC)
                select NewCompare_HISTID,TEST_CASE_ID, STORYBOARD_DETAIL_ID,TEST_BEGIN_TIME,TEST_END_TIME,TEST_RESULT_IN_TEXT,TEST_MODE,TEST_RESULT,TESTER_ID,CREATE_TIME,NewComp_LATEST_TEST_MARK_ID,RELY_TEST_CASE_ID,RESULT_ALIAS_NAME,'Manual' 
                from t_proj_test_result where HIST_ID = OldCompare_HISTID;
		
		        SELECT SEQ_TESTRESULT_ID.nextval INTO NewCompare_REPORTID FROM DUAL; 
                insert into t_test_report (TEST_REPORT_ID,TEST_CASE_ID,LOOP_ID,BEGIN_TIME,END_TIME,RUNNING_RESULT,RETURN_VALUES,RUNNING_RESULT_INFO,HIST_ID,APPLICATION_ID,TEST_MODE)
                select NewCompare_REPORTID,TEST_CASE_ID,LOOP_ID,BEGIN_TIME,END_TIME,RUNNING_RESULT,RETURN_VALUES,RUNNING_RESULT_INFO,NewCompare_HISTID,APPLICATION_ID,TEST_MODE 
                from t_test_report where HIST_ID = OldCompare_HISTID;
		
		        SELECT test_report_id into OldCompare_REPORTID from t_test_report where hist_id= OldCompare_HISTID;
		        insert into t_test_report_steps (TEST_REPORT_STEP_ID,TEST_REPORT_ID,STEPS_ID,BEGIN_TIME,END_TIME,RUNNING_RESULT,RETURN_VALUES,RUNNING_RESULT_INFO,DATA_SUMMARY_ID,INPUT_VALUE_SETTING,ACTUAL_INPUT_DATA,DATA_ORDER,INFO_PIC,ADVICE,STACKINFO)
                select SEQ_TEST_REPORT_STEPS.nextval,NewCompare_REPORTID,STEPS_ID,BEGIN_TIME,END_TIME,RUNNING_RESULT,RETURN_VALUES,RUNNING_RESULT_INFO,DATA_SUMMARY_ID,INPUT_VALUE_SETTING,ACTUAL_INPUT_DATA,DATA_ORDER,INFO_PIC,ADVICE,STACKINFO
                from t_test_report_steps where test_report_id = OldCompare_REPORTID;
		
		        insert into REL_TEST_REPORT_STEPS_COMMENT(ID,test_report_step_id,test_mode,"COMMENT")
                select T_REPORT_STEPS_COMMENT_SEQ.nextval,newreportstep.test_report_step_id,0,relcomment."COMMENT" from t_test_report_steps steps
                inner join REL_TEST_REPORT_STEPS_COMMENT relcomment on steps.test_report_step_id = relcomment.test_report_step_id
                inner join(select * from t_test_report_steps where test_report_id = NewCompare_REPORTID) newreportstep 
                on nvl(newreportstep.input_value_setting,'null1') = nvl(steps.input_value_setting,'null1') and nvl(newreportstep.steps_Id,0) = nvl(steps.steps_Id,0)
                where steps.test_report_id = OldCompare_REPORTID and relcomment.test_mode = 0 and relcomment."COMMENT" is not null;
            END;
        END IF;

		IF OldBaseline_HISTID != 0 THEN
            BEGIN
              SELECT SEQ_TESTRESULT_ID.nextval INTO NewBase_LATEST_TEST_MARK_ID FROM DUAL;
              SELECT SEQ_TESTRESULT_ID.nextval INTO NewBaseline_HISTID FROM DUAL;
		
              insert into t_proj_test_result (HIST_ID,TEST_CASE_ID, STORYBOARD_DETAIL_ID,TEST_BEGIN_TIME,TEST_END_TIME,TEST_RESULT_IN_TEXT,TEST_MODE,TEST_RESULT,TESTER_ID,CREATE_TIME,LATEST_TEST_MARK_ID,RELY_TEST_CASE_ID,RESULT_ALIAS_NAME,RESULT_DESC)
              select NewBaseline_HISTID,TEST_CASE_ID, STORYBOARD_DETAIL_ID,TEST_BEGIN_TIME,TEST_END_TIME,TEST_RESULT_IN_TEXT,TEST_MODE,TEST_RESULT,TESTER_ID,CREATE_TIME,NewBase_LATEST_TEST_MARK_ID,RELY_TEST_CASE_ID,RESULT_ALIAS_NAME,'Manual'  
              from t_proj_test_result where HIST_ID = OldBaseline_HISTID;

              SELECT SEQ_TESTRESULT_ID.nextval INTO NewBaseline_REPORTID FROM DUAL; 
              insert into t_test_report (TEST_REPORT_ID,TEST_CASE_ID,LOOP_ID,BEGIN_TIME,END_TIME,RUNNING_RESULT,RETURN_VALUES,RUNNING_RESULT_INFO,HIST_ID,APPLICATION_ID,TEST_MODE)
              select NewBaseline_REPORTID,TEST_CASE_ID,LOOP_ID,BEGIN_TIME,END_TIME,RUNNING_RESULT,RETURN_VALUES,RUNNING_RESULT_INFO,NewBaseline_HISTID,APPLICATION_ID,TEST_MODE 
              from t_test_report where HIST_ID = OldBaseline_HISTID;

     
              SELECT test_report_id into OldBaseline_REPORTID from t_test_report where hist_id= OldBaseline_HISTID;
        
              insert into t_test_report_steps (TEST_REPORT_STEP_ID,TEST_REPORT_ID,STEPS_ID,BEGIN_TIME,END_TIME,RUNNING_RESULT,RETURN_VALUES,RUNNING_RESULT_INFO,DATA_SUMMARY_ID,INPUT_VALUE_SETTING,ACTUAL_INPUT_DATA,DATA_ORDER,INFO_PIC,ADVICE,STACKINFO)
              select SEQ_TEST_REPORT_STEPS.nextval,NewBaseline_REPORTID,STEPS_ID,BEGIN_TIME,END_TIME,RUNNING_RESULT,RETURN_VALUES,RUNNING_RESULT_INFO,DATA_SUMMARY_ID,INPUT_VALUE_SETTING,ACTUAL_INPUT_DATA,DATA_ORDER,INFO_PIC,ADVICE,STACKINFO
              from t_test_report_steps where test_report_id = OldBaseline_REPORTID;
        
              insert into REL_TEST_REPORT_STEPS_COMMENT(ID,test_report_step_id,test_mode,"COMMENT")
              select T_REPORT_STEPS_COMMENT_SEQ.nextval,newreportstep.test_report_step_id,1,relcomment."COMMENT" from t_test_report_steps steps
              inner join REL_TEST_REPORT_STEPS_COMMENT relcomment on steps.test_report_step_id = relcomment.test_report_step_id
              inner join(select * from t_test_report_steps where test_report_id = NewBaseline_REPORTID) newreportstep 
              on  nvl(newreportstep.input_value_setting,'null1') = nvl(steps.input_value_setting,'null1') and nvl(newreportstep.steps_Id,0) = nvl(steps.steps_Id,0)
              where steps.test_report_id = OldBaseline_REPORTID and relcomment.test_mode = 1 and relcomment."COMMENT" is not null;
               END;
            END IF;
            
        END;
        
end SP_SAVEAS_RESULTSET;

/
create or replace procedure SP_GET_STORYBOARD_RESULT(
Compare_HISTID in NUMBER,
Baseline_HISTID in NUMBER,
--StoryboardDetailID in NUMBER,
sl_cursor OUT SYS_REFCURSOR
)
is
stm VARCHAR2(30000);
begin



 

stm := '';
    stm := '
   select baseline.ID,compare.ID, baseline.TEST_REPORT_STEP_ID as BaselineStepID,
   compare.TEST_REPORT_STEP_ID as CompareStepID,
    baseline.test_report_Id as BaselineReportID,
   compare.test_report_Id as CompareReportID,
baseline.steps_id,
baseline.BEGIN_TIME,
baseline.END_TIME,
baseline.RUNNING_RESULT,
baseline.RETURN_VALUES as Baseline_RETURN_VALUES,
compare.RETURN_VALUES as Compare_RETURN_VALUES,
baseline.RUNNING_RESULT_INFO,
COALESCE(baseline.INPUT_VALUE_SETTING,compare.INPUT_VALUE_SETTING) as INPUT_VALUE_SETTING ,
baseline.ACTUAL_INPUT_DATA,
baseline.DATA_ORDER,
baseline.info_pic,
baseline.ADVICE,
baseline.STACKINFO, baseline.key_word_name,
baseline."COMMENT" as BaselineComment ,
compare."COMMENT" as CompareComment,
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
step."COMMENT" as DSCOMMENT
from t_test_report rep
inner join t_test_report_steps repstep on rep.test_report_id=repstep.test_report_id
left join t_test_steps step on repstep.steps_id=step.steps_id
left join t_keyword on step.key_word_id=t_keyword.key_word_id
inner join t_proj_test_result prjres on rep.hist_id=prjres.hist_id
left join REL_TEST_REPORT_STEPS_COMMENT relstepcomment on repstep.TEST_REPORT_STEP_ID = relstepcomment.TEST_REPORT_STEP_ID
    and prjres.test_mode = relstepcomment.test_mode
where prjres.hist_id ='||Baseline_HISTID||'
and (t_keyword.key_word_name in (''CaptureAndCompare'',''CaptureValue'',''CaptureAndCompareByKey'') or repstep.steps_id is null) and prjres.test_mode=1) baseline
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
relstepcomment.ID,
rep.test_report_Id,
step."COMMENT" as DSCOMMENT
from t_test_report rep
inner join t_test_report_steps repstep on rep.test_report_id=repstep.test_report_id
left join t_test_steps step on repstep.steps_id=step.steps_id
left join t_keyword on step.key_word_id=t_keyword.key_word_id
inner join t_proj_test_result prjres on rep.hist_id=prjres.hist_id
left join REL_TEST_REPORT_STEPS_COMMENT relstepcomment on repstep.TEST_REPORT_STEP_ID = relstepcomment.TEST_REPORT_STEP_ID and prjres.test_mode = relstepcomment.test_mode
where prjres.hist_id ='|| Compare_HISTID ||'
and (t_keyword.key_word_name in (''CaptureAndCompare'',''CaptureValue'',''CaptureAndCompareByKey'')
or repstep.steps_id is null) and prjres.test_mode=0 ) compare 
on 
--nvl(baseline.steps_id,0)=nvl(compare.steps_id,0)
--and 
nvl(baseline.input_value_setting,''null1'')=nvl(compare.input_value_setting,''null1'')

 where nvl(baseline.input_value_setting,''null1'') != ''SKIP'' and nvl(compare.input_value_setting,''null1'') != ''SKIP''

order by ColumnOrder
';
     OPEN sl_cursor FOR  stm ; 
end;
/


create or replace PROCEDURE SP_SAVE_SB_RESULTS
(
feeddetailid in varchar2
)AS 
BEGIN

 

UPDATE t_test_report_steps SET (t_test_report_steps.return_values,t_test_report_steps.Input_value_setting)= (
SELECT distinct tblstgsbresult.baselinevalue,tblstgsbresult.INPUTVALUESETTING FROM tblstgsbresult 
  WHERE  tblstgsbresult.feedprocessdetailid=feeddetailid AND tblstgsbresult.baselineid=t_test_report_steps.test_report_step_id
)
 WHERE EXISTS ( SELECT distinct t1.baselinevalue,t1.INPUTVALUESETTING FROM tblstgsbresult t1
                 WHERE t1.baselineid = t_test_report_steps.test_report_step_id AND t1.feedprocessdetailid=feeddetailid);
                 

 

 UPDATE t_test_report_steps SET (t_test_report_steps.return_values,t_test_report_steps.Input_value_setting) = (
SELECT tblstgsbresult.comparevalue,tblstgsbresult.INPUTVALUESETTING FROM tblstgsbresult 
WHERE  tblstgsbresult.feedprocessdetailid=feeddetailid AND tblstgsbresult.compareid=t_test_report_steps.test_report_step_id
)
 WHERE EXISTS ( SELECT distinct t1.comparevalue,t1.INPUTVALUESETTING FROM tblstgsbresult t1
                 WHERE t1.compareid = t_test_report_steps.test_report_step_id AND t1.feedprocessdetailid=feeddetailid);

 

insert into t_test_report_steps (test_report_step_id,test_report_id,return_values,input_value_setting)
select SEQ_TEST_REPORT_STEPS.nextval,COMPAREREPORTID,comparevalue,inputvaluesetting 
from TBLSTGSBRESULT where feedprocessdetailid = feeddetailid and compareid = 0;

 

insert into t_test_report_steps (test_report_step_id,test_report_id,return_values,input_value_setting)
select SEQ_TEST_REPORT_STEPS.nextval,BASELINEREPORTID,baselinevalue,inputvaluesetting 
from TBLSTGSBRESULT where feedprocessdetailid = feeddetailid and baselineid = 0;

 

insert into REL_TEST_REPORT_STEPS_COMMENT(ID,test_report_step_id,test_mode,"COMMENT")
select T_REPORT_STEPS_COMMENT_SEQ.nextval,steps.TEST_REPORT_STEP_ID,1,stg.baselinecomment from TBLSTGSBRESULT stg
join t_test_report_steps steps on nvl(steps.input_value_setting,'null1') = NVL(stg.inputvaluesetting,'null1') and steps.test_report_id = stg.baselinereportid
    and nvl(steps.return_values,'null1') = NVL(stg.baselinevalue,'null1')
where stg.feedprocessdetailid = feeddetailid and stg.baselineid = 0  and stg.baselinecomment is not null   ;

 

insert into REL_TEST_REPORT_STEPS_COMMENT(ID,test_report_step_id,test_mode,"COMMENT")
select T_REPORT_STEPS_COMMENT_SEQ.nextval,steps.TEST_REPORT_STEP_ID,0,stg.comparecomment from TBLSTGSBRESULT stg
join t_test_report_steps steps on nvl(steps.input_value_setting,'null1') = NVL(stg.inputvaluesetting,'null1') and steps.test_report_id = stg.comparereportid
    and nvl(steps.return_values,'null1') = NVL(stg.comparevalue,'null1')
where stg.feedprocessdetailid = feeddetailid  and stg.compareid = 0 and stg.comparecomment is not null  ;

 

insert into REL_TEST_REPORT_STEPS_COMMENT(ID,test_report_step_id,test_mode,"COMMENT")
select T_REPORT_STEPS_COMMENT_SEQ.nextval,stg.baselineid,1,stg.baselinecomment from TBLSTGSBRESULT stg
left join REL_TEST_REPORT_STEPS_COMMENT relcomment on relcomment.test_report_step_id = stg.baselineid
where stg.feedprocessdetailid = feeddetailid and relcomment.Id is null and stg.baselineid <> 0 ;

 

update REL_TEST_REPORT_STEPS_COMMENT relcomment set relcomment."COMMENT"=(
select stg.baselinecomment from TBLSTGSBRESULT stg where relcomment.test_report_step_id = stg.baselineid and relcomment.test_mode=1 and
 stg.feedprocessdetailid = feeddetailid and stg.baselinecomment is not null)
WHERE EXISTS ( SELECT 1 FROM tblstgsbresult stg WHERE stg.baselineid = relcomment.test_report_step_id
AND relcomment.test_mode = 1 AND stg.feedprocessdetailid=feeddetailid and stg.baselinecomment is not null);

 

insert into REL_TEST_REPORT_STEPS_COMMENT(ID,test_report_step_id,test_mode,"COMMENT")
select T_REPORT_STEPS_COMMENT_SEQ.nextval,stg.compareid,0,stg.comparecomment from TBLSTGSBRESULT stg
left join REL_TEST_REPORT_STEPS_COMMENT relcomment on relcomment.test_report_step_id = stg.compareid
where stg.feedprocessdetailid = feeddetailid  and relcomment.Id is null and stg.compareid <> 0;

 

update REL_TEST_REPORT_STEPS_COMMENT relcomment set relcomment."COMMENT"=(
select stg.comparecomment from TBLSTGSBRESULT stg where relcomment.test_report_step_id = stg.compareid and relcomment.test_mode=0 and
 stg.feedprocessdetailid = feeddetailid and stg.comparecomment is not null)
WHERE EXISTS ( SELECT 1 FROM tblstgsbresult stg WHERE stg.compareid = relcomment.test_report_step_id
AND relcomment.test_mode = 0 AND stg.feedprocessdetailid=feeddetailid and stg.comparecomment is not null);

 

update t_test_report_steps steps set steps.steps_id = (
select rstg.steps_id from tblstgsbresult stg
join t_test_report_steps rstg on nvl(stg.baselinevalue,'null1') = nvl(rstg.return_values,'null1') and nvl(stg.inputvaluesetting,'null1')= nvl(rstg.input_value_setting,'null1')
    where stg.feedprocessdetailid= feeddetailid and stg.baselineid > 0 and stg.compareid = 0  and rstg.test_report_step_id = stg.baselineid
    and nvl(steps.return_values,'null1') = nvl(stg.comparevalue,'null1') and nvl(steps.input_value_setting,'null1') = nvl(stg.inputvaluesetting,'null1')  )
where EXISTS 
(select rstg.steps_id from tblstgsbresult stg
join t_test_report_steps rstg on nvl(stg.baselinevalue,'null1') = nvl(rstg.return_values,'null1') and nvl(stg.inputvaluesetting,'null1')= nvl(rstg.input_value_setting,'null1')
    where stg.feedprocessdetailid= feeddetailid and stg.baselineid > 0 and stg.compareid = 0  and rstg.test_report_step_id = stg.baselineid
    and nvl(steps.return_values,'null1') = nvl(stg.comparevalue,'null1') and nvl(steps.input_value_setting,'null1') = nvl(stg.inputvaluesetting,'null1') );
    
    
        
update t_test_report_steps steps set steps.steps_id = (
select rstg.steps_id from tblstgsbresult stg
join t_test_report_steps rstg on nvl(stg.comparevalue,'null1') = nvl(rstg.return_values,'null1') and nvl(stg.inputvaluesetting,'null1')= nvl(rstg.input_value_setting,'null1')
    where stg.feedprocessdetailid= feeddetailid and stg.baselineid = 0 and stg.compareid > 0  and rstg.test_report_step_id = stg.compareid
    and nvl(steps.return_values,'null1') = nvl(stg.baselinevalue,'null1') and nvl(steps.input_value_setting,'null1') = nvl(stg.inputvaluesetting,'null1')  )
where EXISTS 
(select rstg.steps_id from tblstgsbresult stg
join t_test_report_steps rstg on nvl(stg.comparevalue,'null1') = nvl(rstg.return_values,'null1') and nvl(stg.inputvaluesetting,'null1')= nvl(rstg.input_value_setting,'null1')
    where stg.feedprocessdetailid= feeddetailid and stg.baselineid = 0 and stg.compareid > 0  and rstg.test_report_step_id = stg.compareid
    and nvl(steps.return_values,'null1') = nvl(stg.baselinevalue,'null1') and nvl(steps.input_value_setting,'null1') = nvl(stg.inputvaluesetting,'null1')  );

 
END SP_SAVE_SB_RESULTS;

/

create or replace procedure SP_Export_SB_ResultSet(
ResultMode in number,
ProjectName in varchar2,
StoryboardName in varchar2,
sl_cursor OUT SYS_REFCURSOR
)
is
stm VARCHAR2(9000);
begin

stm := '';
stm := '
select distinct
tss.STORYBOARD_NAME,
proj.PROJECT_NAME,
case when prjres.test_mode=1 then ''BASELINE''
    else ''COMPARE'' end as RESULTMODE,
testcase.TEST_CASE_NAME,  
datasummary.ALIAS_NAME as DatasetName,
mgr.RUN_ORDER as ROWNUMBER,
prjres.RESULT_ALIAS_NAME as Name,
prjres.RESULT_DESC as DescriptiTon,
repstep.INPUT_VALUE_SETTING as ObjTag,
repstep.RETURN_VALUES as ObjValue,
prjres.HIST_ID,
repstep.TEST_REPORT_STEP_ID
from t_test_report rep
inner join t_test_report_steps repstep on rep.test_report_id=repstep.test_report_id
left join t_test_steps step on repstep.steps_id=step.steps_id
left join t_keyword on step.key_word_id=t_keyword.key_word_id
inner join t_proj_test_result prjres on rep.hist_id=prjres.hist_id
inner join T_PROJ_TC_MGR mgr on prjres.STORYBOARD_DETAIL_ID = mgr.STORYBOARD_DETAIL_ID
inner join T_STORYBOARD_SUMMARY tss on mgr.STORYBOARD_ID = tss.STORYBOARD_ID
inner join T_TEST_PROJECT proj on mgr.PROJECT_ID = proj.PROJECT_ID
inner join T_TEST_CASE_SUMMARY testcase on prjres.TEST_CASE_ID = testcase.TEST_CASE_ID
inner join T_STORYBOARD_DATASET_SETTING setting on prjres.STORYBOARD_DETAIL_ID = setting.STORYBOARD_DETAIL_ID
inner join T_TEST_DATA_SUMMARY datasummary on setting.DATA_SUMMARY_ID = datasummary.DATA_SUMMARY_ID
 JOIN (
  select max(rslt.LATEST_TEST_MARK_ID) DASH_LATEST_MARK_ID, 
           max(rslt.HIST_ID) HIST_ID_MX,
          rslt.STORYBOARD_DETAIL_ID,
          rslt.TEST_CASE_ID,
          rslt.TEST_MODE
        from T_PROJ_TEST_RESULT rslt
        where           
            rslt.TEST_MODE = '||ResultMode||'
        group by --rslt.HIST_ID,
        rslt.STORYBOARD_DETAIL_ID,rslt.TEST_CASE_ID, RSLT.TEST_MODE   
  ) HISEX
ON prjres.STORYBOARD_DETAIL_ID=HISEX.STORYBOARD_DETAIL_ID
AND prjres.TEST_CASE_ID=HISEX.TEST_CASE_ID
AND prjres.LATEST_TEST_MARK_ID = HISEX.DASH_LATEST_MARK_ID
AND prjres.HIST_ID = HISEX.HIST_ID_MX


where 
--mgr.RUN_ORDER = 6
--and
(t_keyword.key_word_name in (''CaptureAndCompare'',''CaptureValue'',''CaptureAndCompareByKey'') or repstep.steps_id is null) 
and prjres.test_mode = '||ResultMode||'
and upper(proj.PROJECT_NAME) = upper('''||ProjectName||''')
and upper(tss.STORYBOARD_NAME) =  upper('''||StoryboardName||''')
and repstep.INPUT_VALUE_SETTING <> ''SKIP''
order by mgr.RUN_ORDER, repstep.TEST_REPORT_STEP_ID
';
     OPEN sl_cursor FOR  stm; 
end;



/

create or replace procedure SP_Export_Proj_ResultSet(
ResultMode in number,
ProjectName in varchar2,
sl_cursor OUT SYS_REFCURSOR
)
is
stm VARCHAR2(9000);
begin

stm := '';
stm := '
select distinct
tss.STORYBOARD_NAME,
proj.PROJECT_NAME,
case when prjres.test_mode=1 then ''BASELINE''
    else ''COMPARE'' end as RESULTMODE,
testcase.TEST_CASE_NAME,  
datasummary.ALIAS_NAME as DatasetName,
mgr.RUN_ORDER as ROWNUMBER,
prjres.RESULT_ALIAS_NAME as Name,
prjres.RESULT_DESC as DescriptiTon,
repstep.INPUT_VALUE_SETTING as ObjTag,
repstep.RETURN_VALUES as ObjValue,
prjres.HIST_ID,
repstep.TEST_REPORT_STEP_ID
from t_test_report rep
inner join t_test_report_steps repstep on rep.test_report_id=repstep.test_report_id
left join t_test_steps step on repstep.steps_id=step.steps_id
left join t_keyword on step.key_word_id=t_keyword.key_word_id
inner join t_proj_test_result prjres on rep.hist_id=prjres.hist_id
inner join T_PROJ_TC_MGR mgr on prjres.STORYBOARD_DETAIL_ID = mgr.STORYBOARD_DETAIL_ID
inner join T_STORYBOARD_SUMMARY tss on mgr.STORYBOARD_ID = tss.STORYBOARD_ID
inner join T_TEST_PROJECT proj on mgr.PROJECT_ID = proj.PROJECT_ID
inner join T_TEST_CASE_SUMMARY testcase on prjres.TEST_CASE_ID = testcase.TEST_CASE_ID
inner join T_STORYBOARD_DATASET_SETTING setting on prjres.STORYBOARD_DETAIL_ID = setting.STORYBOARD_DETAIL_ID
inner join T_TEST_DATA_SUMMARY datasummary on setting.DATA_SUMMARY_ID = datasummary.DATA_SUMMARY_ID
 JOIN (
  select max(rslt.LATEST_TEST_MARK_ID) DASH_LATEST_MARK_ID, 
           max(rslt.HIST_ID) HIST_ID_MX,
          rslt.STORYBOARD_DETAIL_ID,
          rslt.TEST_CASE_ID,
          rslt.TEST_MODE
        from T_PROJ_TEST_RESULT rslt
        where           
            rslt.TEST_MODE = '||ResultMode||'
        group by --rslt.HIST_ID,
        rslt.STORYBOARD_DETAIL_ID,rslt.TEST_CASE_ID, RSLT.TEST_MODE   
  ) HISEX
ON prjres.STORYBOARD_DETAIL_ID=HISEX.STORYBOARD_DETAIL_ID
AND prjres.TEST_CASE_ID=HISEX.TEST_CASE_ID
AND prjres.LATEST_TEST_MARK_ID = HISEX.DASH_LATEST_MARK_ID
AND prjres.HIST_ID = HISEX.HIST_ID_MX


where 

(t_keyword.key_word_name in (''CaptureAndCompare'',''CaptureValue'',''CaptureAndCompareByKey'') or repstep.steps_id is null) 
and prjres.test_mode = '||ResultMode||'
and upper(proj.PROJECT_NAME) = upper('''||ProjectName||''')
and repstep.INPUT_VALUE_SETTING <> ''SKIP''
order by mgr.RUN_ORDER, repstep.TEST_REPORT_STEP_ID
';
     OPEN sl_cursor FOR  stm; 
end;



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
        ELSIF VALIDATETYPE='STORYBOARDNAME' THEN
            BEGIN
                SELECT COUNT(*) INTO COUNTTESTSUITE FROM T_STORYBOARD_SUMMARY WHERE UPPER(storyboard_name)=UPPER(DATAVALUE);
            END;
        ELSE
            BEGIN
                SELECT COUNT(*) INTO COUNTTESTSUITE FROM T_REGISTERED_APPS WHERE UPPER(T_REGISTERED_APPS.APP_SHORT_NAME)=UPPER(DATAVALUE);

 

            END;
        END IF;

 


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

create or replace PROCEDURE SP_LIST_PROJECTS(
Startrec in number,
totalpagesize in number,
ColumnName in varchar2,
Columnorder in varchar2,
NameSearch in varchar2,
DescriptionSearch in varchar2,
AppSearch in varchar2,
StatusSearch in varchar2,
sl_cursor OUT SYS_REFCURSOR
)
IS
stm VARCHAR2(30000);
begin
DECLARE 
    Columnnamevalue varchar2(200):=NULL;
    WhereClause varchar2(2000):=Null;
    NewColumnName varchar(200):=Null;
Begin
    if lower(ColumnName) = 'name'
    then begin NewColumnName := 'Upper(projectname)';
    end;
    end if;

 

    if lower(ColumnName) = 'description'
    then begin NewColumnName := 'Upper(description)';
    end;
    end if;

 

    if lower(ColumnName) = 'application'
    then begin NewColumnName := 'dbms_lob.substr(Upper(Applicationame), dbms_lob.getlength(Upper(Applicationame)), 1)';
    end;
    end if;

 

    if lower(ColumnName) = 'status'
    then begin NewColumnName := 'projectstatus';
    end;
    end if;
    WhereClause:='projectname is not null';
    IF NameSearch is not null
    then begin Columnnamevalue:='projectname';
        WhereClause:=' (lower(projectname) like lower(''%'||NameSearch||'%''))';
    end;
    end IF;

 

    IF DescriptionSearch is not null
    then begin Columnnamevalue:='description';
        if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:= WhereClause || '  (lower(description) like lower(''%'||DescriptionSearch||'%''))';
    end;
    end IF;

 

    IF AppSearch is not null
    then begin Columnnamevalue:='Applicationame';
     if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:=WhereClause || '  (lower(Applicationame) like lower(''%'||AppSearch||'%''))';
    end;
    end IF;

 

    IF StatusSearch is not null
    then begin Columnnamevalue:='projectstatus';
     if WhereClause is not null then begin WhereClause:= WhereClause || ' and ' ; end; end if;
        WhereClause:=WhereClause || '   (lower(projectstatus) like lower(''%'||StatusSearch||'%''))';
    end;
    end IF;

 

 if WhereClause is not null
 then begin 
 WhereClause:= ' where ' || WhereClause;
 end;
 end if;

 

stm := '';
stm:='
select * from (Select COUNT(*) OVER () RESULT_COUNT, row_number() over (order by '|| NewColumnName ||' '||ColumnOrder||')  as row_num, projectid,projectname,description,applicationid,Applicationame,projectstatus,statusid from(
select  
proj.project_id as projectid,
proj.project_name as projectname,
case when proj.project_description is null then ''''
else proj.project_description end
as description,
(SELECT  concatenate_appid_project(proj.project_id) FROM DUAL) AS applicationid,
(SELECT  concatenate_app_project(proj.project_id) FROM DUAL) AS Applicationame,
case when proj.status =''1'' then ''Edit''
  when proj.status =''2'' then ''Ready to test''
else '''' end as projectstatus,
proj.status as statusid
 from t_test_project proj
 left join rel_app_proj reltapp on proj.project_id=reltapp.project_id
left join t_registered_apps app on reltapp.application_id=app.application_id

 

--where proj.project_name is not null --and '||whereclause||'
--and (lower(proj.project_name) like lower(''%'||namesearch||'%''))
--and (lower(proj.project_description) like lower(''%'||descriptionsearch||'%''))
--and (lower(app.app_short_name) like lower(''%'||appsearch||'%''))
                --and (proj.status='||statussearch||')

 

 group by proj.project_id,proj.project_name,proj.project_description,proj.status

 

)'||whereclause||' order by '|| NewColumnName ||' '||ColumnOrder||'
)
where row_num between '||Startrec||' and '|| (Startrec-1+totalpagesize) ||'

 

';
 DBMS_OUTPUT.PUT_LINE(stm); 
OPEN sl_cursor FOR  stm ;
end;
end SP_LIST_PROJECTS;

/

create or replace PROCEDURE SP_SAVE_SB_IMPORT_RESULTS
(
feeddetailid in varchar2
)AS 
BEGIN
DECLARE
     RMode number:=0;
     CURRENTDATE TIMESTAMP;
     BEGIN
      SELECT Resultmode into RMode from TBLSTGSTORYBOARDRESULT WHERE FEEDPROCESSDETAILID=feeddetailid and ROWNUM <= 1;
      SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

    --------update returen value in resultset start----------------
     update t_test_report_steps trs SET (trs.return_values)= (
      select distinct tbj.OBJVALUE from TBLSTGSTORYBOARDRESULT tbj 
      join T_TEST_PROJECT proj on upper(proj.project_Name) = upper(tbj.projectname)
      join T_STORYBOARD_SUMMARY ts on upper(ts.storyboard_name) = upper(tbj.storyboardname) and ts.assigned_project_id = proj.project_id
      join T_test_case_summary tc on upper(tc.test_case_name) = upper(tbj.TESTCASENAME) 
      join T_proj_tc_mgr tmgr on tmgr.project_id = proj.project_id and tmgr.test_case_id = tc.test_case_id and tmgr.storyboard_id = ts.storyboard_id 
      and tmgr.run_order = tbj.rownumber
      join t_proj_test_result tptr on tptr.test_case_id = tc.test_case_id and tptr.storyboard_detail_id = tmgr.storyboard_detail_id and tptr.test_mode = tbj.RESULTMODE 
      and upper(tptr.RESULT_ALIAS_NAME) = upper(tbj.name)
      join t_test_report tr on tr.test_case_id = tc.test_case_id and tr.test_mode = tbj.RESULTMODE and tr.hist_id = tptr.hist_id 
      where tbj.FEEDPROCESSDETAILID=feeddetailid  and trs.test_report_id = tr.test_report_id and nvl(trs.INPUT_VALUE_SETTING,'null1') = nvl(tbj.OBJTAG,'null1')
      )
      where EXISTS (
      select distinct tbj.OBJVALUE from TBLSTGSTORYBOARDRESULT tbj 
      join T_TEST_PROJECT proj on upper(proj.project_Name) = upper(tbj.projectname)
      join T_STORYBOARD_SUMMARY ts on upper(ts.storyboard_name) = upper(tbj.storyboardname) and ts.assigned_project_id = proj.project_id
      join T_test_case_summary tc on upper(tc.test_case_name) = upper(tbj.TESTCASENAME) 
      join T_proj_tc_mgr tmgr on tmgr.project_id = proj.project_id and tmgr.test_case_id = tc.test_case_id and tmgr.storyboard_id = ts.storyboard_id 
      and tmgr.run_order = tbj.rownumber
     join t_proj_test_result tptr on tptr.test_case_id = tc.test_case_id and tptr.storyboard_detail_id = tmgr.storyboard_detail_id and tptr.test_mode = tbj.RESULTMODE 
      and upper(tptr.RESULT_ALIAS_NAME) = upper(tbj.name)
      join t_test_report tr on tr.test_case_id = tc.test_case_id and tr.test_mode = tbj.RESULTMODE and tr.hist_id = tptr.hist_id 
      where tbj.FEEDPROCESSDETAILID=feeddetailid and  trs.test_report_id = tr.test_report_id and nvl(trs.INPUT_VALUE_SETTING,'null1') = nvl(tbj.OBJTAG,'null1'));

    --------update returen value in resultset end----------------

     ------update resultset description start-------

     update t_proj_test_result tptr SET (tptr.RESULT_DESC)= (
     select DESCRIPTITON from (select distinct tbj.name,tmgr.run_order,tbj.DESCRIPTITON from TBLSTGSTORYBOARDRESULT tbj 
     join T_TEST_PROJECT proj on upper(proj.project_Name) = upper(tbj.projectname)
     join T_STORYBOARD_SUMMARY ts on upper(ts.storyboard_name) = upper(tbj.storyboardname) and ts.assigned_project_id = proj.project_id
     join T_test_case_summary tc on upper(tc.test_case_name) = upper(tbj.TESTCASENAME) 
     join T_proj_tc_mgr tmgr on tmgr.project_id = proj.project_id and tmgr.test_case_id = tc.test_case_id and tmgr.storyboard_id = ts.storyboard_id 
     and tmgr.run_order = tbj.rownumber
     where tbj.FEEDPROCESSDETAILID=feeddetailid and tptr.test_case_id = tc.test_case_id and tptr.storyboard_detail_id = tmgr.storyboard_detail_id and 
     tptr.test_mode = tbj.RESULTMODE  and upper(tptr.RESULT_ALIAS_NAME) = upper(tbj.name)))
     where EXISTS (
     select DESCRIPTITON from (select distinct tbj.name,tmgr.run_order,tbj.DESCRIPTITON from TBLSTGSTORYBOARDRESULT tbj
     join T_TEST_PROJECT proj on upper(proj.project_Name) = upper(tbj.projectname)
     join T_STORYBOARD_SUMMARY ts on upper(ts.storyboard_name) = upper(tbj.storyboardname) and ts.assigned_project_id = proj.project_id
     join T_test_case_summary tc on upper(tc.test_case_name) = upper(tbj.TESTCASENAME) 
     join T_proj_tc_mgr tmgr on tmgr.project_id = proj.project_id and tmgr.test_case_id = tc.test_case_id and tmgr.storyboard_id = ts.storyboard_id 
     and tmgr.run_order = tbj.rownumber
      where tbj.FEEDPROCESSDETAILID=feeddetailid and tptr.test_case_id = tc.test_case_id and tptr.storyboard_detail_id = tmgr.storyboard_detail_id and 
      tptr.test_mode = tbj.RESULTMODE  and upper(tptr.RESULT_ALIAS_NAME) = upper(tbj.name)));

     ------update resultset description end-------
       
      -------add new tag and value in resut set start-------------- 
      
      insert into t_test_report_steps (test_report_step_id,test_report_id,return_values,input_value_setting)
      select SEQ_TEST_REPORT_STEPS.nextval,test_report_id,OBJVALUE,OBJTAG from (
      select tbj.OBJTAG,tbj.OBJVALUE, tr.test_report_id from TBLSTGSTORYBOARDRESULT tbj 
      join T_TEST_PROJECT proj on upper(proj.project_Name) = upper(tbj.projectname)
      join T_STORYBOARD_SUMMARY ts on upper(ts.storyboard_name) = upper(tbj.storyboardname) and ts.assigned_project_id = proj.project_id
      join T_test_case_summary tc on upper(tc.test_case_name) = upper(tbj.TESTCASENAME) 
      join T_proj_tc_mgr tmgr on tmgr.project_id = proj.project_id and tmgr.test_case_id = tc.test_case_id and tmgr.storyboard_id = ts.storyboard_id 
      and tmgr.run_order = tbj.rownumber
      join t_proj_test_result tptr on tptr.test_case_id = tc.test_case_id and tptr.storyboard_detail_id = tmgr.storyboard_detail_id and tptr.test_mode = tbj.RESULTMODE 
      and upper(tptr.RESULT_ALIAS_NAME) = upper(tbj.name)
      join t_test_report tr on tr.test_case_id = tc.test_case_id and tr.test_mode = tbj.RESULTMODE and tr.hist_id = tptr.hist_id 
      left join t_test_report_steps trs on trs.test_report_id = tr.test_report_id and nvl(trs.INPUT_VALUE_SETTING,'null1') = nvl(tbj.OBJTAG,'null1')
      where tbj.FEEDPROCESSDETAILID=feeddetailid  and RESULT_ALIAS_NAME is not null and trs.INPUT_VALUE_SETTING IS NULL and tbj.OBJTAG Is not null);

      -------add new tag and value in resut set end--------------

      ------create new result set start---------------

     insert into t_proj_test_result (HIST_ID,TEST_CASE_ID,STORYBOARD_DETAIL_ID,TEST_RESULT_IN_TEXT,TEST_MODE,CREATE_TIME,LATEST_TEST_MARK_ID,RESULT_ALIAS_NAME,RESULT_DESC)
     select SEQ_TESTRESULT_ID.nextval,test_case_id,storyboard_detail_id,'SUCCESS',resultmode,CURRENTDATE,SEQ_TESTRESULT_ID.nextval,Name,DESCRIPTITON from(
     select distinct  tc.test_case_id,tmgr.storyboard_detail_id,tbj.resultmode,tbj.Name,tbj.DESCRIPTITON from TBLSTGSTORYBOARDRESULT tbj 
      join T_TEST_PROJECT proj on upper(proj.project_Name) = upper(tbj.projectname)
      join T_STORYBOARD_SUMMARY ts on upper(ts.storyboard_name) = upper(tbj.storyboardname) and ts.assigned_project_id = proj.project_id
      join T_test_case_summary tc on upper(tc.test_case_name) = upper(tbj.TESTCASENAME) 
      join T_proj_tc_mgr tmgr on tmgr.project_id = proj.project_id and tmgr.test_case_id = tc.test_case_id and tmgr.storyboard_id = ts.storyboard_id 
      and tmgr.run_order = tbj.rownumber
      left join t_proj_test_result tptr on tptr.test_case_id = tc.test_case_id and tptr.storyboard_detail_id = tmgr.storyboard_detail_id and tptr.test_mode = tbj.RESULTMODE 
      and upper(tptr.RESULT_ALIAS_NAME) = upper(tbj.name)
      where tbj.FEEDPROCESSDETAILID=feeddetailid and tptr.hist_id is null);


     insert into t_test_report (TEST_REPORT_ID,TEST_CASE_ID,LOOP_ID,HIST_ID,TEST_MODE)
     select SEQ_TESTRESULT_ID.nextval,test_case_id,1,hist_id,resultmode from
     (select distinct tc.test_case_id,tmgr.storyboard_detail_id,tbj.resultmode,tptr.hist_id from TBLSTGSTORYBOARDRESULT tbj 
      join T_TEST_PROJECT proj on upper(proj.project_Name) = upper(tbj.projectname)
      join T_STORYBOARD_SUMMARY ts on upper(ts.storyboard_name) = upper(tbj.storyboardname) and ts.assigned_project_id = proj.project_id
      join T_test_case_summary tc on upper(tc.test_case_name) = upper(tbj.TESTCASENAME) 
      join T_proj_tc_mgr tmgr on tmgr.project_id = proj.project_id and tmgr.test_case_id = tc.test_case_id and tmgr.storyboard_id = ts.storyboard_id 
      and tmgr.run_order = tbj.rownumber
      join t_proj_test_result tptr on tptr.test_case_id = tc.test_case_id and tptr.storyboard_detail_id = tmgr.storyboard_detail_id and tptr.test_mode = tbj.RESULTMODE 
      and upper(tptr.RESULT_ALIAS_NAME) = upper(tbj.name)
      left join t_test_report tr on tr.test_case_id = tc.test_case_id and tr.test_mode = tbj.RESULTMODE and tr.hist_id = tptr.hist_id
      where tbj.FEEDPROCESSDETAILID=feeddetailid and tr.test_report_id is null and tptr.TEST_RESULT_IN_TEXT = 'SUCCESS');

     insert into t_test_report_steps (TEST_REPORT_STEP_ID,TEST_REPORT_ID,RETURN_VALUES,INPUT_VALUE_SETTING)
     select SEQ_TEST_REPORT_STEPS.nextval,test_report_id,objvalue,objtag from (
     select tr.test_report_id,tbj.objvalue,tbj.objtag  from TBLSTGSTORYBOARDRESULT tbj 
     join T_TEST_PROJECT proj on upper(proj.project_Name) = upper(tbj.projectname)
      join T_STORYBOARD_SUMMARY ts on upper(ts.storyboard_name) = upper(tbj.storyboardname) and ts.assigned_project_id = proj.project_id
      join T_test_case_summary tc on upper(tc.test_case_name) = upper(tbj.TESTCASENAME) 
      join T_proj_tc_mgr tmgr on tmgr.project_id = proj.project_id and tmgr.test_case_id = tc.test_case_id and tmgr.storyboard_id = ts.storyboard_id 
      and tmgr.run_order = tbj.rownumber
      join t_proj_test_result tptr on tptr.test_case_id = tc.test_case_id and tptr.storyboard_detail_id = tmgr.storyboard_detail_id and tptr.test_mode = tbj.RESULTMODE 
      and upper(tptr.RESULT_ALIAS_NAME) = upper(tbj.name)
      join t_test_report tr on tr.test_case_id = tc.test_case_id and tr.test_mode = tbj.RESULTMODE and tr.hist_id = tptr.hist_id
      left join t_test_report_steps trs  on  trs.test_report_id = tr.test_report_id
      where tbj.FEEDPROCESSDETAILID=feeddetailid and trs.test_report_id is null and tptr.TEST_RESULT_IN_TEXT = 'SUCCESS' order by tbj.ROWSID);

    ------create new result set end---------------


   END;
END SP_SAVE_SB_IMPORT_RESULTS;



/


create or replace procedure SP_VALIDATE_IMPORT_RESULTS(
fprocessdetailid in varchar2,
 RESULT OUT CLOB
)
is
begin
DECLARE
CHECKRESULT  NUMBER:=0;
begin
            -------Project not found start------
DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);
    BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=31;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=31;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT distinct (SELECT SYSDATE FROM DUAL),
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Project does not exist',
                   ERROR,
                   SA.STORYBOARDNAME,
                   SA.PROJECTNAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM TBLSTGSTORYBOARDRESULT sa
            LEFT JOIN T_TEST_PROJECT proj ON upper(SA.PROJECTNAME)=upper(PROJ.PROJECT_NAME) 
            WHERE SA.FEEDPROCESSDETAILID=fprocessdetailid
                  and PROJ.PROJECT_ID is null 
                  and SA.PROJECTNAME is not null;

    END;

    -------Project not found end------
    -------Storyboard not found start------
    DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=32;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=32;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT distinct (SELECT SYSDATE FROM DUAL),
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Storyboard does not exist',
                   ERROR,
                   SA.STORYBOARDNAME,
                   SA.PROJECTNAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM TBLSTGSTORYBOARDRESULT sa
            LEFT JOIN T_STORYBOARD_SUMMARY sb ON upper(SA.STORYBOARDNAME)=upper(SB.STORYBOARD_NAME) 
            WHERE SA.FEEDPROCESSDETAILID=fprocessdetailid
                  and SB.STORYBOARD_NAME is null 
                  and SA.STORYBOARDNAME is not null;

    END;
    -------Storyboard not found end------
    -------Testcase not found start------
     DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=34;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=34;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,TESTCASENAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT distinct (SELECT SYSDATE FROM DUAL),
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Test case does not exist',
                   ERROR,
                   SA.STORYBOARDNAME,
                   SA.PROJECTNAME,
                   SA.TESTCASENAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM TBLSTGSTORYBOARDRESULT sa
            LEFT JOIN T_TEST_CASE_SUMMARY tc ON upper(SA.TESTCASENAME)=upper(TC.TEST_CASE_NAME) 
            WHERE SA.FEEDPROCESSDETAILID=fprocessdetailid
                  and TC.TEST_CASE_NAME is null 
                  and SA.TESTCASENAME is not null;

    END;
    -------Testcase not found end------

    -------Dataset not found start------
     DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=35;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=35;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,TESTCASENAME,DATASETNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT distinct (SELECT SYSDATE FROM DUAL),
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Data set does not exist',
                   ERROR,
                   SA.STORYBOARDNAME,
                   SA.PROJECTNAME,
                   SA.TESTCASENAME,
                   SA.DATASETNAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM TBLSTGSTORYBOARDRESULT sa
            LEFT JOIN T_TEST_DATA_SUMMARY td ON upper(SA.DATASETNAME)=upper(TD.ALIAS_NAME) 
            WHERE SA.FEEDPROCESSDETAILID=fprocessdetailid
                  and TD.ALIAS_NAME is null 
                  and SA.DATASETNAME is not null;

    END;
    -------Dataset not found end------

    -------Project and storyboard linking start------

      DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=55;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=55;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,TESTCASENAME,DATASETNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT distinct (SELECT SYSDATE FROM DUAL),
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Storyboard does not belong to given project',
                   ERROR,
                   SA.STORYBOARDNAME,
                   SA.PROJECTNAME,
                   '',
                   '',
                   ERROR,
                   '',
                   fprocessdetailid
            FROM TBLSTGSTORYBOARDRESULT sa
            JOIN T_TEST_PROJECT proj on upper(SA.PROJECTNAME)=upper(PROJ.PROJECT_NAME)
            LEFT JOIN T_STORYBOARD_SUMMARY sb on upper(SA.STORYBOARDNAME)=upper(SB.STORYBOARD_NAME) and SB.ASSIGNED_PROJECT_ID=PROJ.PROJECT_ID
            WHERE SB.ASSIGNED_PROJECT_ID IS NULL AND SA.FEEDPROCESSDETAILID=fprocessdetailid;

    END;
    -------Project and storyboard linking end------

    -------Testcase and Dataset linking start------
     DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=39;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=39;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,TESTCASENAME,DATASETNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT (SELECT SYSDATE FROM DUAL),
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Dataset is not linked with testcase',
                   ERROR,
                   SA.STORYBOARDNAME,
                   SA.PROJECTNAME,
                   SA.TESTCASENAME,
                   SA.DATASETNAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM TBLSTGSTORYBOARDRESULT sa
            JOIN T_TEST_CASE_SUMMARY tc ON upper(SA.TESTCASENAME)=upper(TC.TEST_CASE_NAME)
            JOIN T_TEST_DATA_SUMMARY td ON upper(SA.DATASETNAME)=upper(TD.ALIAS_NAME)
            LEFT JOIN REL_TC_DATA_SUMMARY reltc ON TC.TEST_CASE_ID=RELTC.TEST_CASE_ID 
                                                AND TD.DATA_SUMMARY_ID=RELTC.DATA_SUMMARY_ID
            WHERE RELTC.id is null AND SA.FEEDPROCESSDETAILID=fprocessdetailid;

    END;
    -------Testcase and Dataset linking end------

    -------Testcase, Storyboard and Project linking start------
        DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=56;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=56;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,TESTCASENAME,DATASETNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT distinct (SELECT SYSDATE FROM DUAL),
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Test case does not belong to storyboard',
                   ERROR,
                   SA.STORYBOARDNAME,
                   SA.PROJECTNAME,
                   SA.TESTCASENAME,
                   SA.DATASETNAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM TBLSTGSTORYBOARDRESULT sa
            JOIN T_TEST_PROJECT proj ON upper(SA.PROJECTNAME)=upper(PROJ.PROJECT_NAME)
            JOIN T_STORYBOARD_SUMMARY sb ON upper(SA.STORYBOARDNAME)=upper(SB.STORYBOARD_NAME) AND SB.ASSIGNED_PROJECT_ID=PROJ.PROJECT_ID
            JOIN T_TEST_CASE_SUMMARY tc ON TC.TEST_CASE_NAME=SA.TESTCASENAME
            LEFT JOIN T_PROJ_TC_MGR mgr ON MGR.PROJECT_ID= PROJ.PROJECT_ID 
                                        AND MGR.STORYBOARD_ID=SB.STORYBOARD_ID 
                                        AND MGR.TEST_CASE_ID=TC.TEST_CASE_ID
            WHERE MGR.STORYBOARD_DETAIL_ID IS NULL AND SA.FEEDPROCESSDETAILID=fprocessdetailid;

    END; 

     -------Testcase, Storyboard and Project linking end------

     -------Invalid rownumber start------
     DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=57;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=57;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,TESTCASENAME,DATASETNAME,
                                      TESTSTEPNUMBER,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT distinct (SELECT SYSDATE FROM DUAL),
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Rownumber is invalid',
                   ERROR,
                   SA.STORYBOARDNAME,
                   SA.PROJECTNAME,
                   SA.TESTCASENAME,
                   SA.DATASETNAME,
                   '',
                   ERROR,
                   '',
                   fprocessdetailid
            FROM TBLSTGSTORYBOARDRESULT sa
          where ( SA.PROJECTNAME,SA.STORYBOARDNAME,
                   SA.TESTCASENAME,
                   SA.DATASETNAME,
                   SA.ROWNUMBER) not in(select   SA.PROJECTNAME,SA.STORYBOARDNAME,
                   SA.TESTCASENAME,
                   SA.DATASETNAME,
                   SA.ROWNUMBER   FROM TBLSTGSTORYBOARDRESULT sa
            JOIN T_TEST_PROJECT proj ON upper(SA.PROJECTNAME)=upper(PROJ.PROJECT_NAME)
            JOIN T_STORYBOARD_SUMMARY sb ON upper(SA.STORYBOARDNAME)=upper(SB.STORYBOARD_NAME) AND SB.ASSIGNED_PROJECT_ID=PROJ.PROJECT_ID
            JOIN T_TEST_CASE_SUMMARY tc ON upper(TC.TEST_CASE_NAME)=upper(SA.TESTCASENAME)
            JOIN T_TEST_DATA_SUMMARY td ON upper(sa.datasetname)=upper(td.alias_name)
            JOIN T_PROJ_TC_MGR mgr ON MGR.PROJECT_ID= PROJ.PROJECT_ID 
                                        AND MGR.STORYBOARD_ID=SB.STORYBOARD_ID
                                        AND MGR.TEST_CASE_ID=TC.TEST_CASE_ID
                                        AND mgr.run_order=SA.ROWNUMBER
             JOIN T_STORYBOARD_DATASET_SETTING setting ON MGR.STORYBOARD_DETAIL_ID = SETTING.STORYBOARD_DETAIL_ID 
                                                          AND TD.DATA_SUMMARY_ID=SETTING.DATA_SUMMARY_ID where SA.FEEDPROCESSDETAILID=fprocessdetailid)
             AND SA.FEEDPROCESSDETAILID=fprocessdetailid;

    END; 
     -------Invalid rownumber end------

     -------Project name is different in sheets end------
     DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=56;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=56;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,TESTCASENAME,DATASETNAME,
                                      TESTSTEPNUMBER,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Project name is different in sheets',
                   ERROR,
                   '',
                   projectname,
                   '',
                   '',
                   '',
                   ERROR,
                   '',
                   fprocessdetailid
            FROM (  
              SELECT projectname FROM (
               SELECT Rownum as ID,projectname FROM (
                  SELECT projectname 
                  FROM tblstgstoryboardresult
                  WHERE FEEDPROCESSDETAILID =fprocessdetailid
                  GROUP BY projectname )) 
                WHERE ID > 1  )
              GROUP BY CURRENTDATE,ERROR_TYPE,'Validation','','Project name is different in sheets',ERROR,'',projectname,'','','',ERROR,'',fprocessdetailid;

    END; 
     -------Project name is different in sheets end------

     -------Duplicate object tag in resultset start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=58;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=58;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,TESTCASENAME,DATASETNAME,
                                      TESTSTEPNUMBER,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,Objectname)

            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Object tag is different',
                   ERROR,
                   storyboardname,
                   projectname,
                   testcasename,
                   datasetname,
                   '',
                   ERROR,
                   '',
                   fprocessdetailid,
                   objtag
            FROM (  
            SELECT objtag,
                   RESULTMODE,
                   projectname,
                   STORYBOARDNAME,
                   TESTCASENAME,
                   DATASETNAME,
                   ROWNUMBER 
            FROM TBLSTGSTORYBOARDRESULT 
            WHERE FEEDPROCESSDETAILID=fprocessdetailid
            GROUP BY OBJTAG, RESULTMODE,projectname,STORYBOARDNAME,TESTCASENAME,DATASETNAME,ROWNUMBER
            HAVING COUNT(objtag)>1
              )
           GROUP BY CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Object tag is different',
                   ERROR,
                   storyboardname,
                   projectname,
                   testcasename,
                   datasetname,
                   rownumber,
                   ERROR,
                   '',
                   fprocessdetailid,objtag;

    END; 
     -------Duplicate object tag in resultset end-----

     -------Baseline/Compare start---------------
    DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=59;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=59;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,TESTCASENAME,DATASETNAME,
                                      TESTSTEPNUMBER,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID,General)

            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Mode is different in sheets',
                   ERROR,
                   '',
                   projectname,
                   '',
                   '',
                   '',
                   ERROR,
                   '',
                   fprocessdetailid,
                   case when resultmode=1 then 'Mode:- Baseline' 
                   else 'Mode:- Compare' end
            FROM (  
             SELECT Resultmode,Projectname FROM (
               SELECT Rownum as ID,Resultmode,Projectname FROM (
                  SELECT Resultmode, Projectname
                  FROM tblstgstoryboardresult
                  WHERE FEEDPROCESSDETAILID =fprocessdetailid
                  GROUP BY Resultmode,Projectname )) 
                WHERE ID > 1  )
              GROUP BY CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Mode is different in sheets',
                   ERROR,
                   '',
                   projectname,
                   '',
                   '',
                   '',
                   ERROR,
                   '',
                   fprocessdetailid,
                   resultmode;

    END; 
     -------Baseline/Compare start---------------

     -------Row number duplicated start---------
  DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=57;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=57;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,STORYBOARDNAME,PROJECTNAME,TESTCASENAME,DATASETNAME,
                                      TESTSTEPNUMBER,ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Rownumber is same for multiple datasets',
                   ERROR,
                   storyboardname,
                   projectname,
                   '',
                   '',
                   rownumber,
                   ERROR,
                   '',
                   fprocessdetailid

            FROM (  
            SELECT t.Projectname,t.rownumber,t.storyboardname,T.Testcasename,T.Datasetname
                  FROM tblstgstoryboardresult t

                  WHERE t.FEEDPROCESSDETAILID =fprocessdetailid
                  GROUP BY t.Projectname,t.rownumber,t.storyboardname,T.Testcasename,T.Datasetname)
                  GROUP BY CURRENTDATE,
                           ERROR_TYPE,
                            'Validation',
                            '',
                            'Rownumber is same for multiple datasets',
                             ERROR,
                            storyboardname,
                            projectname,
                            '',
                            '',
                            rownumber,
                            ERROR,
                            '',
                            fprocessdetailid
                  HAVING COUNT(rownumber)>1;

    END; 
    -------Row number duplicated end---------
     SELECT COUNT(*) into CHECKRESULT
    from tblLOGREPORT 
    left JOIN tblfeedprocessdetails ON TBLFEEDPROCESSDETAILS.FEEDPROCESSDETAILID = TBLLOGREPORT.FEEDPROCESSINGDETAILSID
    where 
         tblLOGREPORT.FEEDPROCESSINGDETAILSID=  FPROCESSDETAILID;

IF CHECKRESULT != 0 THEN
    RESULT:= 'ERROR';
END IF; 
End;
end ;



/

create or replace PROCEDURE "SP_EXPORT_LOGREPORT_RESULT" (
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
    left JOIN tblfeedprocessdetails ON TBLFEEDPROCESSDETAILS.FEEDPROCESSDETAILID = TBLLOGREPORT.FEEDPROCESSINGDETAILSID
    where 
         tblLOGREPORT.FEEDPROCESSINGDETAILSID= ' || FEEDPROCESSDETAILID || ' ORDER BY 9, 10, 1';
    OPEN sl_cursor FOR  stm ;
    END;

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

create or replace PROCEDURE          "SP_EXPORT_EXPORTDATATAGFOLDER" (
sl_cursor OUT SYS_REFCURSOR
)
IS
	stm VARCHAR2(30000);

BEGIN

    stm := 'select folderid, foldername,description ,active from t_test_folder';
    OPEN sl_cursor FOR  stm ;

END SP_EXPORT_EXPORTDATATAGFOLDER;

/

create or replace PROCEDURE          "SP_EXPORT_EXPORTDATATAGGROUP" (
sl_cursor OUT SYS_REFCURSOR
)
IS
	stm VARCHAR2(30000);

BEGIN

    stm := 'select groupid, groupname,description,active from t_test_group';
    OPEN sl_cursor FOR  stm ;

END SP_EXPORT_EXPORTDATATAGGROUP;

/

create or replace PROCEDURE          "SP_EXPORT_EXPORTDATASETTAGSET" (
sl_cursor OUT SYS_REFCURSOR
)
IS
	stm VARCHAR2(30000);

BEGIN

    stm := 'select setid, setname,description, active from T_test_set';
    OPEN sl_cursor FOR  stm ;

END SP_EXPORT_EXPORTDATASETTAGSET;

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

create or replace procedure SP_VALIDATE_IMPORT_DATASETTAG(
fprocessdetailid in varchar2,
 RESULT OUT CLOB
)
is
begin
DECLARE
CHECKRESULT  NUMBER:=0;
begin
            -------Dataset not found start------
DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);
    BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=63;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=63;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,DATASETNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT distinct (SELECT SYSDATE FROM DUAL),
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Dataset Name does not exist',
                   ERROR,
                   SA.DATASETNAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM TBLSTGDATASETTAG sa
            LEFT JOIN T_Test_data_summary dtg ON upper(SA.DATASETNAME)=upper(dtg.alias_name) 
            WHERE SA.FEEDPROCESSDETAILID=fprocessdetailid
                  and dtg.data_summary_id is null 
                  and SA.DATASETNAME is not null;

    END;

    -------Dataset not found end------
   -------Duplicate Dataset in Sheet start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=64;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=64;
            SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,DATASETNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Dataset Name is Duplicate',
                   ERROR,
                   DATASETNAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM (  
            SELECT DATASETNAME
            FROM TBLSTGDATASETTAG 
            WHERE FEEDPROCESSDETAILID=fprocessdetailid
            GROUP BY DATASETNAME
            HAVING COUNT(DATASETNAME)>1
              )
           GROUP BY CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Dataset Name is Duplicate',
                   ERROR,
                   DATASETNAME,
                   ERROR,
                   '',
                   fprocessdetailid;

    END; 
     -------Duplicate Dataset in Sheet end-----

       -------Duplicate group in Sheet start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=65;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=65;
            SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,DATASETNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Group Name is Duplicate',
                   ERROR,
                   NAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM (  
            SELECT NAME
            FROM TBLSTGCOMMONDATASETTAG 
            WHERE FEEDPROCESSDETAILID=fprocessdetailid and type='Group'
            GROUP BY NAME
            HAVING COUNT(NAME)>1
              )
           GROUP BY CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Group Name is Duplicate',
                   ERROR,
                   NAME,
                   ERROR,
                   '',
                   fprocessdetailid;

    END; 
     -------Duplicate group in Sheet end-----


       -------Duplicate set in Sheet start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=66;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=66;
            SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,DATASETNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Set Name is Duplicate',
                   ERROR,
                   NAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM (  
            SELECT NAME
            FROM TBLSTGCOMMONDATASETTAG 
            WHERE FEEDPROCESSDETAILID=fprocessdetailid and type='Set'
            GROUP BY NAME
            HAVING COUNT(NAME)>1
              )
           GROUP BY CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Set Name is Duplicate',
                   ERROR,
                   NAME,
                   ERROR,
                   '',
                   fprocessdetailid;

    END; 
     -------Duplicate set in Sheet end-----

       -------Duplicate folder in Sheet start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=67;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=67;
            SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,DATASETNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Folder Name is Duplicate',
                   ERROR,
                   NAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM (  
            SELECT NAME
            FROM TBLSTGCOMMONDATASETTAG 
            WHERE FEEDPROCESSDETAILID=fprocessdetailid and type='Folder'
            GROUP BY NAME
            HAVING COUNT(NAME)>1
              )
           GROUP BY CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Folder Name is Duplicate',
                   ERROR,
                   NAME,
                   ERROR,
                   '',
                   fprocessdetailid;

    END; 
     -------Duplicate folder in Sheet end-----

    -------Group not found start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=60;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=60;
            SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,OBJECTNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)
            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Group does not exist',
                   ERROR,
                   tsg.taggroup,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM  TBLSTGDATASETTAG tsg 
            left join 
            (SELECT distinct tg.groupname as name  FROM  (select * from TBLSTGCOMMONDATASETTAG where type='Group' and FEEDPROCESSDETAILID=fprocessdetailid) tbl
             right join t_test_group tg ON upper(tbl.name) = upper(tg.groupname)
            union
            SELECT distinct tbl.name as name  FROM  (select * from TBLSTGCOMMONDATASETTAG where type='Group' and FEEDPROCESSDETAILID=fprocessdetailid) tbl
            left join t_test_group tg ON upper(tbl.name) = upper(tg.groupname)) tmp  ON upper(tmp.name) = upper(tsg.taggroup)where tsg.FEEDPROCESSDETAILID=fprocessdetailid 
            and tmp.name is  null and tsg.taggroup is not null;

    END; 
     -------Group not found end-----

      -------Set not found start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=61;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=61;
            SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,OBJECTNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)
            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Set does not exist',
                   ERROR,
                   tsg.tagset,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM  TBLSTGDATASETTAG tsg 
            left join 
            (SELECT distinct ts.setname as name  FROM  (select * from TBLSTGCOMMONDATASETTAG where type='Set' and FEEDPROCESSDETAILID=fprocessdetailid) tbl
             right join t_test_set ts ON upper(tbl.name) = upper(ts.setname)
             union
            SELECT distinct tbl.name as name  FROM  (select * from TBLSTGCOMMONDATASETTAG where type='Set' and FEEDPROCESSDETAILID=fprocessdetailid) tbl
            left join t_test_set ts ON upper(tbl.name) = upper(ts.setname)) tmp  ON upper(tmp.name) = upper(tsg.tagset)where tsg.FEEDPROCESSDETAILID=fprocessdetailid 
            and tmp.name is  null and tsg.tagset is not null;

    END; 
     -------Set not found end-----

      -------Folder not found start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=62;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=62;
            SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,OBJECTNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)
            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Folder does not exist',
                   ERROR,
                   tsg.tagfolder,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM  TBLSTGDATASETTAG tsg 
            left join 
            (SELECT distinct tf.foldername as name  FROM  (select * from TBLSTGCOMMONDATASETTAG where type='Folder' and FEEDPROCESSDETAILID=fprocessdetailid) tbl
             right join t_test_folder tf ON upper(tbl.name) = upper(tf.foldername)
            union
            SELECT distinct tbl.name as name  FROM  (select * from TBLSTGCOMMONDATASETTAG where type='Folder' and FEEDPROCESSDETAILID=fprocessdetailid) tbl
            left join t_test_folder tf ON upper(tbl.name) = upper(tf.foldername)) tmp  ON upper(tmp.name) = upper(tsg.tagfolder)where tsg.FEEDPROCESSDETAILID=fprocessdetailid 
            and tmp.name is  null and tsg.tagfolder is not null;

    END; 
     -------Folder not found end-----
  -------Duplicate folder/Sequence start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=68;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=68;
            SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,DATASETNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Folder Name and their Sequence is Duplicate',
                   ERROR,
                   foldername,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM (  
            SELECT tagfolder as foldername, SEQUENCE
            FROM TBLSTGDATASETTAG 
            WHERE FEEDPROCESSDETAILID=fprocessdetailid
            GROUP BY tagfolder, SEQUENCE
            HAVING COUNT(*)>1
              )
           GROUP BY CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Folder Name and their Sequence is Duplicate',
                   ERROR,
                   foldername,
                   ERROR,
                   '',
                   fprocessdetailid;

    END; 
     -------Duplicate folder/Sequence end-----

     -------Group not active in sheet start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=69;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=69;
            SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,OBJECTNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)
            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Group is not active in sheet',
                   ERROR,
                   taggroup,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM (
             select distinct stg.taggroup ,stg1.active from TBLSTGDATASETTAG stg
           join TBLSTGCOMMONDATASETTAG stg1 on STG.taggroup =STG1.name and STG1.type='Group' and stg1.FEEDPROCESSDETAILID=fprocessdetailid
           where STG.FEEDPROCESSDETAILID=fprocessdetailid and STG1.ACTIVE=0);

    END; 
     -------Group not active in sheet end-----

  -------Set not active in sheet start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=70;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=70;
            SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,OBJECTNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)
            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Set is not active in sheet',
                   ERROR,
                   tagset,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM (
             select distinct stg.tagset ,stg1.active from TBLSTGDATASETTAG stg
           join TBLSTGCOMMONDATASETTAG stg1 on STG.TAGSET =STG1.name and STG1.type='Set' and stg1.FEEDPROCESSDETAILID=fprocessdetailid
           where STG.FEEDPROCESSDETAILID=fprocessdetailid and STG1.ACTIVE=0);

    END; 
     -------Set not active in sheet end-----

       -------Folder not active in sheet start-----
   DECLARE
        ERROR VARCHAR2(500);
        ERROR_TYPE VARCHAR2(500);
        CURRENTDATE TIMESTAMP;
     BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=71;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=71;
            SELECT SYSDATE INTO CURRENTDATE FROM DUAL;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,OBJECTNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)
            SELECT CURRENTDATE,
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'Folder is not active in sheet',
                   ERROR,
                   tagfolder,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM (
                select distinct stg.tagfolder ,stg1.active from TBLSTGDATASETTAG stg
           join TBLSTGCOMMONDATASETTAG stg1 on STG.tagfolder =STG1.name and STG1.type='Folder' and stg1.FEEDPROCESSDETAILID=fprocessdetailid
           where STG.FEEDPROCESSDETAILID=fprocessdetailid and STG1.ACTIVE=0);

    END; 
     -------Folder not active in sheet end-----
     
    -------sequence must be greater than 0 start------
DECLARE
        ERROR VARCHAR2(500);
		ERROR_TYPE VARCHAR2(500);
    BEGIN
            SELECT MESSAGE INTO ERROR FROM TBLMESSAGE WHERE ID=72;
            SELECT messagesymbol INTO ERROR_TYPE FROM TBLMESSAGE WHERE ID=72;

            INSERT INTO TBLLOGREPORT(CREATIONDATE,MESSAGETYPE,ACTION,
                                      CELLADDRESS,VALIDATIONNAME,VALIDATIONDESCRIPTION,DATASETNAME,
                                      ERRORDESCRIPTION,PROGRAMLOCATION,FEEDPROCESSINGDETAILSID)

            SELECT distinct (SELECT SYSDATE FROM DUAL),
                   ERROR_TYPE,
                   'Validation',
                   '',
                   'sequence must be greater than 0',
                   ERROR,
                   DATASETNAME,
                   ERROR,
                   '',
                   fprocessdetailid
            FROM TBLSTGDATASETTAG where FEEDPROCESSDETAILID=fprocessdetailid and sequence <= 0;

    END;

    -------sequence must be greater than 0 end------
     SELECT COUNT(*) into CHECKRESULT
    from tblLOGREPORT 
    left JOIN tblfeedprocessdetails ON TBLFEEDPROCESSDETAILS.FEEDPROCESSDETAILID = TBLLOGREPORT.FEEDPROCESSINGDETAILSID
    where 
         tblLOGREPORT.FEEDPROCESSINGDETAILSID=  FPROCESSDETAILID;

IF CHECKRESULT != 0 THEN
    RESULT:= 'ERROR';
END IF; 
End;
end;
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


create or replace procedure SP_GET_USER_CONFIGURATION(
sl_cursor OUT SYS_REFCURSOR
)
IS 
stm VARCHAR2(30000);

BEGIN

stm := 'select tpr.USERCONFIGID, nvl(tpr.MAINKEY, '''') as MAINKEY ,nvl(tpr.SUBKEY, '''') as SUBKEY, tpr.USERID,
nvl(tp.TESTER_LOGIN_NAME, '''') as MARSUserName,
utl_raw.cast_to_varchar2(dbms_lob.substr(tpr.BLOBVALUE)) as BLOBValuestr,
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