CREATE TABLE "TBLSTGSTORYBOARDVALID"
   (    "ROW_ID" NUMBER,
    "RUN_ORDER" NUMBER,
    "PROJECTID" NUMBER,
    "ACTIONAME" VARCHAR2(20 BYTE),
    "STEPNAME" VARCHAR2(500 BYTE),
    "TESTSUITENAME" VARCHAR2(500 BYTE),
    "TESTCASENAME" VARCHAR2(500 BYTE),
    "DATASETNAME" VARCHAR2(500 BYTE),
    "DEPENDENCY" VARCHAR2(500 BYTE),
    "FEEDPROCESSID" NUMBER,
    "FEEDPROCESSDETAILID" NUMBER,
    "STORYBOARDDETAILID" NVARCHAR2(10),
    "STORYBOARDID" VARCHAR2(20 BYTE)
   ) 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT);
  
  /
  
    CREATE TABLE "TBLSTGSTORYBOARDVALIDATION" 
   (    "ID" NUMBER, 
    "VALIDATIONMSG" VARCHAR2(2000 BYTE), 
    "ISVALID" NUMBER, 
    "FEEDPROCESSID" NUMBER, 
    "FEEDPROCESSDETAILID" NUMBER
   ) 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
  NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT);
  /

  CREATE TABLE "T_USER_CONFIGURATION" 
   (	"USERCONFIGID" NUMBER(38,0) NOT NULL ENABLE, 
	"MAINKEY" VARCHAR2(128 BYTE) NOT NULL ENABLE, 
	"SUBKEY" VARCHAR2(128 BYTE), 
	"USERID" NUMBER(38,0), 
	"BLOBVALUE" BLOB, 
	"BLOBVALUETYPE" NUMBER(2,0), 
	"DESCRIPTION" VARCHAR2(256 BYTE), 
	"CREATEDBY" VARCHAR2(50 BYTE), 
	"CREATEDON" DATE, 
	"MODIFYBY" VARCHAR2(50 BYTE), 
	"MODIFYON" DATE, 
	"ISDELETE" NUMBER(2,0), 
	 CONSTRAINT "T_USER_CONFIGURATION_PK" PRIMARY KEY ("USERCONFIGID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 
    ENABLE
   ) 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
   ;
  
  /
  
  CREATE SEQUENCE  "T_SEQ_USERCONFIG"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 81 CACHE 20 NOORDER  CYCLE;
  
  /
  INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Add UserConfiguration', 'UserConfiguration','Add UserConfiguration',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Add UserConfiguration');

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Edit UserConfiguration', 'UserConfiguration','Edit UserConfiguration',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Edit UserConfiguration');

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Delete UserConfiguration', 'UserConfiguration','Delete UserConfiguration',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Delete UserConfiguration');

/

 ALTER TABLE TBLSTGDATASETTAG
 add DESCRIPTION varchar2(512);
  
 commit;
 /
 
 
alter table t_test_datasettag modify EXPECTEDRESULTS VARCHAR2(4000);	

alter table t_test_datasettag modify STEPDESC VARCHAR2(4000);	

alter table t_test_datasettag modify DIARY VARCHAR2(4000);	

alter table TBLSTGDATASETTAG modify EXPECTEDRESULTS VARCHAR2(4000);	

alter table TBLSTGDATASETTAG modify STEPDESC VARCHAR2(4000);	

alter table TBLSTGDATASETTAG modify DIARY VARCHAR2(4000);	

commit;

/

  CREATE TABLE "T_DATABASE_CONNECTIONS" 
   (	"CONNECTION_ID" NUMBER(16,0) NOT NULL ENABLE, 
	"CONNECTION_NAME" VARCHAR2(128 BYTE), 
	"CONNECTION_TYPE" NUMBER(2,0), 
	"HOST_NAME" VARCHAR2(128 BYTE), 
	"PORT_NUMBER" NUMBER(8,0), 
	"PROTOCOL" VARCHAR2(16 BYTE), 
	"SERVICE_NAME" VARCHAR2(64 BYTE), 
	"DB_SID" VARCHAR2(64 BYTE), 
	"DB_USERNAME" VARCHAR2(32 BYTE), 
	"DB_PASSWORD" VARCHAR2(32 BYTE), 
	"CONNECTION_STRING" VARCHAR2(1028 BYTE), 
	"ACTIVE" NUMBER(2,0), 
	"CREATEDBY" VARCHAR2(64 BYTE), 
	"CREATION_DATE" DATE, 
	"MODIFIEDBY" VARCHAR2(64 BYTE), 
	"MODIFIED_DATE" DATE, 
	"IS_TESTED" NUMBER(2,0), 
	"LAST_TESTED" DATE, 
	"ERROR_MESSAGE" VARCHAR2(1028 BYTE), 
	 CONSTRAINT "T_DATABASE_CONNECTIONS_PK" PRIMARY KEY ("CONNECTION_ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
   ) 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT);
 
 /
 
   CREATE SEQUENCE  "T_DATABASE_CONNECTIONS_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 81 CACHE 20 NOORDER  CYCLE;
/




  CREATE TABLE "T_QUERY" 
   (	"QUERY_ID" NUMBER(16,0) NOT NULL ENABLE, 
	"QUERY_NAME" VARCHAR2(128 BYTE), 
	"QUERY_DESC" VARCHAR2(1028 BYTE), 
	"IS_ACTIVE" NUMBER(2,0), 
	"CREATEDBY" VARCHAR2(64 BYTE), 
	"CREATED_DATE" DATE, 
	"MODIFIEDBY" VARCHAR2(64 BYTE), 
	"MODIFIED_DATE" DATE, 
	"CONN_ID" NUMBER(16,0), 
	 CONSTRAINT "T_QUERYID_PK" PRIMARY KEY ("QUERY_ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT), 
	 CONSTRAINT "T_QUERY_FK1" FOREIGN KEY ("CONN_ID")
	  REFERENCES "T_DATABASE_CONNECTIONS" ("CONNECTION_ID") ON DELETE CASCADE ENABLE
   ) 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT);


 
 /
  CREATE SEQUENCE  "T_QUERY_SEQ"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 81 CACHE 20 NOORDER  CYCLE;
 /
 
  
  CREATE TABLE "T_AXIS_LIST" 
   (	"AXIS_LIST_ID" NUMBER(16,0), 
	"QUERY_ID" NUMBER(16,0), 
	"COMMAND_TYPE_ID" NUMBER(2,0), 
	"X_AXIS" NUMBER(2,0), 
	"Y_AXIS" NUMBER(2,0), 
	"Z_AXIS" NUMBER(2,0), 
	 PRIMARY KEY ("AXIS_LIST_ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 , 
	 CONSTRAINT "T_AXIS_LIST_FK1" FOREIGN KEY ("QUERY_ID")
	  REFERENCES "T_QUERY" ("QUERY_ID") ON DELETE CASCADE ENABLE
   ) 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING;
 
/

 CREATE SEQUENCE  "SEQ_T_AXIS_LIST"  MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 81 CACHE 20 NOORDER  CYCLE;
 
 /
 
 ALTER TABLE TBLSTGTESTCASESAVE
 add ParentObj varchar2(100);
 /
 alter table system_lookup modify display_name VARCHAR2(256);
 
 /
 INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Import CompareConfig', 'CompareConfig','Import CompareConfig',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Import CompareConfig');

 

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Export CompareConfig', 'CompareConfig','Export CompareConfig',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Export CompareConfig');

 

commit;

/

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Add Connection', 'Database Connection','Add Connection',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Add Connection');

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Edit Connection', 'Database Connection','Edit Connection',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Edit Connection');

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'View Connection', 'Database Connection','View Connection',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'View Connection');

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Delete Connection', 'Database Connection','Delete Connection',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Delete Connection');

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Add Query', 'Query','Add Query',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Add Query');

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Edit Query', 'Query','Edit Query',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Edit Query');

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'View Query', 'Query','View Query',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'View Query');

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Delete Query', 'Query','Delete Query',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Delete Query');

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Add Axis', 'Display Chart','Add Axis',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Add Axis');

INSERT INTO T_TEST_PRIVILEGE(PRIVILEGE_ID,PRIVILEGE_NAME,MODULE,DESCRIPTION,ISACTIVE,CREATOR,CREATOR_DATE)
SELECT SEQ_T_TEST_PRIVILEGE.NEXTVAL,'Display Chart', 'Display Chart','Display Chart',1,1,SYSDATE FROM dual 
WHERE NOT EXISTS (select * from T_TEST_PRIVILEGE where PRIVILEGE_NAME = 'Display Chart');

commit;
/

ALTER TABLE T_DATABASE_CONNECTIONS MODIFY SERVICE_NAME VARCHAR2(128);

commit;

/

 ALTER TABLE T_DBCONNECTION
 add DATABASE_VALUE BLOB;
 
 commit;
 
 /
 
 ALTER TABLE T_QUERY
DROP CONSTRAINT T_QUERY_FK1;

commit;