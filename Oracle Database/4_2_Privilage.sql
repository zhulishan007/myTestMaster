
---Only assign User Role
declare lRoleId Number;
begin
select Role_ID into lRoleId from T_TEST_ROLES where upper(ROLE_NAME) = 'USER';



insert into T_TEST_USER_ROLE_MAPPING (USER_ROLE_MAP_ID,User_ID,ROLE_ID,ISACTIVE,CREATOR,CREATOR_DATE)
select REL_T_TEST_USER_ROLE_MAPPING.nextval,TESTER_ID, lRoleId,1,'Admin',(SELECT SYSDATE FROM DUAL)
from t_tester_info tester where upper(TESTER_LOGIN_NAME) != 'ADMIN' ;
commit;
end;