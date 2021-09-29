create or replace FUNCTION concatenate_datasetids(test_case_id1 NUMERIC, data_set_id Numeric)
  RETURN CLOB
IS
  l_return CLOB; 
BEGIN

  if data_set_id > 0 then
  begin
  SELECT DISTINCT RTRIM(XMLAGG(XMLELEMENT(E,reltcds.DATA_SUMMARY_ID,',').EXTRACT('//text()') ORDER BY ALIAS_NAME).GetClobVal(),',') INTO l_return
  FROM REL_TC_DATA_SUMMARY reltcds 
  INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
  WHERE reltcds.TEST_CASE_ID = test_case_id1 and reltcds.DATA_SUMMARY_ID = data_set_id;
end;
else
begin

SELECT DISTINCT RTRIM(XMLAGG(XMLELEMENT(E,reltcds.DATA_SUMMARY_ID,',').EXTRACT('//text()') ORDER BY ALIAS_NAME).GetClobVal(),',') INTO l_return
  FROM REL_TC_DATA_SUMMARY reltcds 
  INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
  WHERE reltcds.TEST_CASE_ID = test_case_id1;


end;
end if;
  RETURN l_return;

END;

/

create or replace FUNCTION concatenate_datasetnames(test_case_id1 NUMERIC, data_set_id Numeric)
  RETURN CLOB
IS
  l_return CLOB; 
BEGIN

 if data_set_id > 0 then
  begin
  SELECT DISTINCT RTRIM(XMLAGG(XMLELEMENT(E,replace(ALIAS_NAME,',','___'),',').EXTRACT('//text()') ORDER BY ALIAS_NAME).GetClobVal(),',') INTO l_return
  FROM REL_TC_DATA_SUMMARY reltcds 
  INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
  WHERE reltcds.TEST_CASE_ID = test_case_id1 and reltcds.DATA_SUMMARY_ID = data_set_id;
end;
else
begin
 SELECT DISTINCT RTRIM(XMLAGG(XMLELEMENT(E,replace(ALIAS_NAME,',','___'),',').EXTRACT('//text()') ORDER BY ALIAS_NAME).GetClobVal(),',') INTO l_return
  FROM REL_TC_DATA_SUMMARY reltcds 
  INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
  WHERE reltcds.TEST_CASE_ID = test_case_id1;



end;
end if;
  RETURN l_return;

END;

/

create or replace FUNCTION concatenate_datasetdescription(test_case_id1 NUMERIC, data_set_id Numeric)
  RETURN CLOB
IS
  l_return  CLOB; 
BEGIN

 if data_set_id > 0 then
  begin
    SELECT RTRIM(XMLAGG(XMLELEMENT(E,x.DESCRIPTION_INFO,',').EXTRACT('//text()') ORDER BY x.alias_name).GetClobVal(),',') INTO l_return
  --SELECT LISTAGG(x.DESCRIPTION_INFO, ',') WITHIN GROUP (ORDER BY ALIAS_NAME) INTO l_return
  FROM (
        SELECT DISTINCT ttds.ALIAS_NAME,NVL(REPLACE(ttds.DESCRIPTION_INFO, ',', '~'), ' ') AS DESCRIPTION_INFO
        FROM REL_TC_DATA_SUMMARY reltcds 
        INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
        WHERE reltcds.TEST_CASE_ID = test_case_id1 and reltcds.DATA_SUMMARY_ID = data_set_id
        ORDER BY ttds.ALIAS_NAME
      ) x;
      
   end;
else
begin
  SELECT RTRIM(XMLAGG(XMLELEMENT(E,x.DESCRIPTION_INFO,',').EXTRACT('//text()') ORDER BY x.alias_name).GetClobVal(),',') INTO l_return
  --SELECT LISTAGG(x.DESCRIPTION_INFO, ',') WITHIN GROUP (ORDER BY ALIAS_NAME) INTO l_return
  FROM (
        SELECT DISTINCT ttds.ALIAS_NAME,NVL(REPLACE(ttds.DESCRIPTION_INFO, ',', '~'), ' ') AS DESCRIPTION_INFO
        FROM REL_TC_DATA_SUMMARY reltcds 
        INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
        WHERE reltcds.TEST_CASE_ID = test_case_id1
        ORDER BY ttds.ALIAS_NAME
      ) x;


end;
end if;
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
        SELECT distinct ttds.ALIAS_NAME,
          (
            SELECT distinct REPLACE(testds.DATA_VALUE, ',', '~') AS ALIAS_NAME
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


create or replace FUNCTION concatenate_teststepskip(test_case_id1 NUMERIC, steps_id1 NUMERIC, data_set_id Numeric)
  RETURN CLOB
IS
  l_return  CLOB; 
BEGIN


if data_set_id > 0 then
  begin
  
  SELECT LISTAGG(x.DATA_DIRECTION, ',') WITHIN GROUP (ORDER BY ALIAS_NAME) INTO l_return
  FROM (
        SELECT  ttds.ALIAS_NAME,NVL(testds.DATA_DIRECTION, 0) AS DATA_DIRECTION
        FROM REL_TC_DATA_SUMMARY reltcds
        INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
        INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID
        LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
          AND testds.steps_id = ts1.STEPS_ID
        WHERE reltcds.TEST_CASE_ID = test_case_id1
          AND ts1.STEPS_ID = steps_id1 and reltcds.DATA_SUMMARY_ID = data_set_id
        ORDER BY ttds.ALIAS_NAME 
      ) x;
      
      
      
      end;
else
begin
  SELECT LISTAGG(x.DATA_DIRECTION, ',') WITHIN GROUP (ORDER BY ALIAS_NAME) INTO l_return
  FROM (
        SELECT  ttds.ALIAS_NAME,NVL(testds.DATA_DIRECTION, 0) AS DATA_DIRECTION
        FROM REL_TC_DATA_SUMMARY reltcds
        INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
        INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID
        LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
          AND testds.steps_id = ts1.STEPS_ID
        WHERE reltcds.TEST_CASE_ID = test_case_id1
          AND ts1.STEPS_ID = steps_id1
        ORDER BY ttds.ALIAS_NAME 
      ) x;
      


end;
end if;
  RETURN l_return;

END;

/


create or replace FUNCTION concatenate_teststepdatasetId(test_case_id1 NUMERIC, steps_id1 NUMERIC, data_set_id Numeric)
  RETURN CLOB
IS
  l_return  CLOB; 
BEGIN

if data_set_id > 0 then
  begin
  
  SELECT RTRIM(XMLAGG(XMLELEMENT(E,Data_Setting_Id,',').EXTRACT('//text()') ORDER BY ALIAS_NAME).GetClobVal(),',') INTO l_return
  FROM (
        SELECT  ttds.ALIAS_NAME,REPLACE(testds.Data_Setting_Id, ',', '~') AS Data_Setting_Id
        FROM REL_TC_DATA_SUMMARY reltcds
        INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
        INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID
        LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
          AND testds.steps_id = ts1.STEPS_ID
        WHERE reltcds.TEST_CASE_ID = test_case_id1
          AND ts1.STEPS_ID = steps_id1 and  reltcds.DATA_SUMMARY_ID = data_set_id
        ORDER BY ttds.ALIAS_NAME 
      ) x;
      
      
      
      end;
else
begin
 SELECT RTRIM(XMLAGG(XMLELEMENT(E,Data_Setting_Id,',').EXTRACT('//text()') ORDER BY ALIAS_NAME).GetClobVal(),',') INTO l_return
  FROM (
        SELECT  ttds.ALIAS_NAME,REPLACE(testds.Data_Setting_Id, ',', '~') AS Data_Setting_Id
        FROM REL_TC_DATA_SUMMARY reltcds
        INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
        INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID
        LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
          AND testds.steps_id = ts1.STEPS_ID
        WHERE reltcds.TEST_CASE_ID = test_case_id1
          AND ts1.STEPS_ID = steps_id1
        ORDER BY ttds.ALIAS_NAME 
      ) x;


end;
end if;
  RETURN l_return;

END;

/

create or replace FUNCTION concatenate_teststepdatavalue(test_case_id1 NUMERIC, steps_id1 NUMERIC, data_set_id Numeric)
  RETURN CLOB
IS
  l_return  CLOB; 
BEGIN
if data_set_id > 0 then
  begin
  
  SELECT RTRIM(XMLAGG(XMLELEMENT(E,DATA_VALUE,',').EXTRACT('//text()') ORDER BY ALIAS_NAME).GetClobVal(),',') INTO l_return
  FROM (
        SELECT  ttds.ALIAS_NAME,REPLACE(testds.DATA_VALUE, ',', '~') AS DATA_VALUE
        FROM REL_TC_DATA_SUMMARY reltcds
        INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
        INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID
        LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
          AND testds.steps_id = ts1.STEPS_ID
        WHERE reltcds.TEST_CASE_ID = test_case_id1
          AND ts1.STEPS_ID = steps_id1 and reltcds.DATA_SUMMARY_ID = data_set_id
        ORDER BY ttds.ALIAS_NAME 
      ) x;
      
  end;
else
begin

 SELECT RTRIM(XMLAGG(XMLELEMENT(E,DATA_VALUE,',').EXTRACT('//text()') ORDER BY ALIAS_NAME).GetClobVal(),',') INTO l_return
  FROM (
        SELECT  ttds.ALIAS_NAME,REPLACE(testds.DATA_VALUE, ',', '~') AS DATA_VALUE
        FROM REL_TC_DATA_SUMMARY reltcds
        INNER JOIN T_TEST_DATA_SUMMARY ttds ON ttds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
        INNER JOIN T_TEST_STEPS ts1 ON ts1.TEST_CASE_ID = reltcds.TEST_CASE_ID
        LEFT JOIN TEST_DATA_SETTING testds ON testds.DATA_SUMMARY_ID = reltcds.DATA_SUMMARY_ID
          AND testds.steps_id = ts1.STEPS_ID
        WHERE reltcds.TEST_CASE_ID = test_case_id1
          AND ts1.STEPS_ID = steps_id1
        ORDER BY ttds.ALIAS_NAME 
      ) x;


end;
end if;
  RETURN l_return;

END;

/


create or replace function MARS_STR_CAPANDCOMP(input_value_setting in varchar2) return varchar2 is
  ipos int; 
  isWith_ varchar2(128);
  prefix varchar2(128) ;
  trail varchar2(128) ;
  rslt varchar2(128) ;
  n number;
begin
    select decode(INSTR(input_value_setting, '_', -1), null,'F', 0, 'F', 'T') into isWith_ from dual ;
    if isWith_<>'T' then
        return input_value_setting ;
    end if;
    ipos :=  INSTR(input_value_setting, '_', -1) ;
    prefix := substr(input_value_setting,1, iPos) ;  
    trail := substr(input_value_setting, iPos+1) ; 
    n := to_number(trail) ;

    rslt := concat(prefix, lpad( trail, 3, '0')) ;
    return rslt ;
exception
  when others then
    return input_value_setting;
end MARS_STR_CAPANDCOMP ;

/

  CREATE OR REPLACE FORCE EDITIONABLE VIEW V_TEST_DATA_REPORT_SUMMARY
  AS 
  SELECT 
     K.KEY_WORD_NAME,OBJ.OBJECT_HAPPY_NAME,
-- version 3-1,2018,1.6
-- VERSION 9-1,21, for alex, arrange input_value_settting to right order
--STP_RPT.INPUT_VALUE_SETTING,
     MARS_STR_CAPANDCOMP(STP_RPT.INPUT_VALUE_SETTING) INPUT_VALUE_SETTING,
     STP_RPT.RETURN_VALUES,
     STP_RPT.ACTUAL_INPUT_DATA,
     STP_RPT_LST.LATEST_TEST_MARK_ID,
     STP_RPT_LST.STORYBOARD_DETAIL_ID,
     STP_RPT_LST.LOOP_ID,
     STP.RUN_ORDER,
     STP_RPT_LST.TEST_MODE,
     STP_RPT.TEST_REPORT_STEP_ID,
     STP.COLUMN_ROW_SETTING,
     STP_RPT_LST.RUN_ORDER STORYBOARD_ORDER,
     STP_RPT_LST.STORYBOARD_ID,
     STP.TEST_CASE_ID,
     STP_RPT.RUNNING_RESULT_INFO,
     STP_RPT.STEPS_ID,
     STP_RPT.INFO_PIC
FROM   
  T_TEST_REPORT_STEPS STP_RPT
-- GET TEST STORY BOARD INFO
LEFT JOIN 
  ( --GET LATEST REPPORT ID
    SELECT STB_LST_HIST.LATEST_TEST_MARK_ID,  
           STB_LST_HIST.STORYBOARD_DETAIL_ID,
           STB_LST_HIST.RUN_ORDER,
           STB_LST_HIST.STORYBOARD_ID,
           TST_RPT.*
    FROM  T_TEST_REPORT TST_RPT,
        V_MARS_STB_SNAPSHOT STB_LST_HIST ,
        (
            SELECT R.STORYBOARD_DETAIL_ID,R.TEST_CASE_ID,R.TEST_MODE,
            MAX(R.HIST_ID) MX_HIST
            FROM T_PROJ_TEST_RESULT R
            GROUP BY R.STORYBOARD_DETAIL_ID,R.TEST_CASE_ID,R.TEST_MODE
        ) MX_HIST_ID
    WHERE TST_RPT.HIST_ID=STB_LST_HIST.HIST_ID
    AND MX_HIST_ID.MX_HIST=TST_RPT.HIST_ID
    AND MX_HIST_ID.STORYBOARD_DETAIL_Id=STB_LST_HIST.STORYBOARD_DETAIL_Id    
    AND MX_HIST_ID.TEST_MODE=STB_LST_HIST.TEST_MODE
  ) STP_RPT_LST
  -- get test case and loop result info
ON STP_RPT.TEST_REPORT_ID=STP_RPT_LST.TEST_REPORT_ID,  
  T_TEST_STEPS STP
LEFT JOIN -- GET KEYWORD NAME
  T_KEYWORD K
ON STP.KEY_WORD_ID = K.KEY_WORD_ID
LEFT JOIN -- GET OBJECT HAPPYNAME
  V_OBJECT_SNAPSHOT OBJ
ON STP.OBJECT_ID=OBJ.OBJECT_ID 
WHERE
-- GET KEYWORD NAME
   STP_RPT.STEPS_ID=STP.STEPS_ID
AND STP_RPT_LST.LATEST_TEST_MARK_ID IS NOT NULL;

