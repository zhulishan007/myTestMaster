
--admin Role assign to Admin user
declare lCount Number;
begin
select count(*) into lCount from T_TEST_USER_ROLE_MAPPING map where
map.USER_ID in (select TESTER_ID from t_tester_info tester where upper(TESTER_LOGIN_NAME)= 'ADMIN')
and map.ROLE_ID in (select trole.ROLE_ID from T_TEST_ROLES trole where upper(trole.ROLE_NAME) = 'ADMIN');



if (lCount = 0)
then
begin
insert into T_TEST_USER_ROLE_MAPPING (USER_ROLE_MAP_ID,User_ID,ROLE_ID,ISACTIVE,CREATOR,CREATOR_DATE)
select REL_T_TEST_USER_ROLE_MAPPING.nextval, (select TESTER_ID from t_tester_info tester where upper(TESTER_LOGIN_NAME)= 'ADMIN'), (select trole.ROLE_ID from T_TEST_ROLES trole where upper(trole.ROLE_NAME) = 'ADMIN'),
1,'Admin',(SELECT SYSDATE FROM DUAL) from DUAL;
commit;
end;
end if;
end;
