CREATE TABLE Authentication_Server (
    Server_ID VARCHAR2(20) PRIMARY KEY,
    IP_address VARCHAR2(20) NOT NULL
);

CREATE TABLE Server_User (
    Login VARCHAR2(20) PRIMARY KEY,
    Email VARCHAR2(20) NOT NULL,
    Password VARCHAR2(20) NOT NULL,
    User_IP_address VARCHAR2(20) NOT NULL,
    Server_ID VARCHAR2(20) NOT NULL,
    CONSTRAINT fk_user FOREIGN KEY (Server_ID) 
    REFERENCES Authentication_Server(Server_ID)
);


CREATE TABLE User_Role (
    User_Role VARCHAR2(20) PRIMARY KEY,
    Server_ID VARCHAR2(20) NOT NULL,
    CONSTRAINT fk_role FOREIGN KEY (Server_ID) 
    REFERENCES Authentication_Server(Server_ID)
);



CREATE TABLE Token_access (
    Token_ID VARCHAR2(20) PRIMARY KEY,
    Validity INTEGER NOT NULL CONSTRAINT Check_1 CHECK (Validity >= 0),
    Access_Permission VARCHAR2(20) NOT NULL,
    Server_ID VARCHAR2(20) NOT NULL,
    CONSTRAINT fk_token_ip FOREIGN KEY (Server_ID) 
    REFERENCES Authentication_Server(Server_ID)
);


CREATE TABLE Server_Session (
    Session_ID INTEGER PRIMARY KEY,
    Time_start INTEGER NOT NULL CONSTRAINT Check_2 CHECK (Time_start >= 0),
    Time_end INTEGER NOT NULL CONSTRAINT Check_3 CHECK (Time_end >= 0),
    Token_ID VARCHAR2(20) NOT NULL,
    Server_ID VARCHAR2(20) NOT NULL,
    CONSTRAINT fk_session_token FOREIGN KEY (Token_ID) 
    REFERENCES Token_access(Token_ID),
    CONSTRAINT fk_session_server FOREIGN KEY (Server_ID) 
    REFERENCES Authentication_Server(Server_ID)
);


CREATE VIEW User_View AS
SELECT su.Login, su.Email, su.User_IP_address, a.ip_address
FROM Server_User su
JOIN Authentication_Server a ON su.Server_ID = a.Server_ID;

CREATE VIEW User_Role_View AS
SELECT ur.User_Role, a.Server_ID, a.IP_address
FROM User_Role ur
JOIN Authentication_Server a ON ur.Server_ID = a.Server_ID;

CREATE VIEW Token_Access_View AS
SELECT t.Token_ID, t.Validity, t.Access_Permission, a.Server_ID, a.IP_address
FROM Token_access t
JOIN Authentication_Server a ON t.Server_ID = a.Server_ID;

CREATE VIEW Server_Session_View AS
SELECT ss.Session_ID, ss.Time_start, ss.Time_end, t.Token_ID, t.validity
FROM Server_Session ss
JOIN Token_access t ON ss.Token_ID = t.Token_ID;

CREATE OR REPLACE PACKAGE auth_server_pkg IS
    -- Процедури для таблиці Authentication_Server
    PROCEDURE insert_authentication_server(p_server_id IN VARCHAR2, p_ip_address IN VARCHAR2);
    PROCEDURE update_authentication_server(p_server_id IN VARCHAR2, p_ip_address IN VARCHAR2);
    PROCEDURE delete_authentication_server(p_server_id IN VARCHAR2);
    FUNCTION get_authentication_server(p_server_id IN VARCHAR2) RETURN VARCHAR2;

    -- Процедури для таблиці User_Role
    PROCEDURE insert_user_role(p_user_role IN VARCHAR2, p_server_id IN VARCHAR2);
    PROCEDURE update_user_role(p_user_role IN VARCHAR2, p_server_id IN VARCHAR2);
    PROCEDURE delete_user_role(p_user_role IN VARCHAR2);

    -- Процедури для таблиці Token_access
    PROCEDURE insert_token_access(p_token_id IN VARCHAR2, p_validity IN INTEGER, p_access_permission IN VARCHAR2, p_server_id IN VARCHAR2);
    PROCEDURE update_token_access(p_token_id IN VARCHAR2, p_validity IN INTEGER, p_access_permission IN VARCHAR2, p_server_id IN VARCHAR2);
    PROCEDURE delete_token_access(p_token_id IN VARCHAR2);
    
    -- Процедури для таблиці Server_Session
    PROCEDURE insert_server_session(p_session_id IN INTEGER, p_time_start IN INTEGER, p_time_end IN INTEGER, p_token_id IN VARCHAR2, p_server_id IN VARCHAR2);
    PROCEDURE delete_server_session(p_session_id IN INTEGER);
    
END auth_server_pkg;
/
CREATE OR REPLACE PACKAGE user_pkg IS

    -- Процедури для таблиці Server_User
    PROCEDURE insert_server_user(p_login IN VARCHAR2, p_email IN VARCHAR2, p_password IN VARCHAR2, p_user_ip IN VARCHAR2, p_server_id IN VARCHAR2);
    PROCEDURE update_server_user(p_login IN VARCHAR2, p_email IN VARCHAR2, p_password IN VARCHAR2, p_user_ip IN VARCHAR2, p_server_id IN VARCHAR2);
    PROCEDURE delete_server_user(p_login IN VARCHAR2);
    FUNCTION get_server_user(p_login IN VARCHAR2) RETURN VARCHAR2;
   
END user_pkg;
/

CREATE OR REPLACE PACKAGE BODY auth_server_pkg IS
    
    -- Процедури для таблиці Authentication_Server
    PROCEDURE insert_authentication_server(p_server_id IN VARCHAR2, p_ip_address IN VARCHAR2) IS
    BEGIN
        INSERT INTO Authentication_Server (Server_ID, IP_address) 
        VALUES (p_server_id, p_ip_address);
    END insert_authentication_server;
    
    PROCEDURE update_authentication_server(p_server_id IN VARCHAR2, p_ip_address IN VARCHAR2) IS
    BEGIN
        UPDATE Authentication_Server
        SET IP_address = p_ip_address
        WHERE Server_ID = p_server_id;
    END update_authentication_server;
    
    PROCEDURE delete_authentication_server(p_server_id IN VARCHAR2) IS
    BEGIN
        DELETE FROM Authentication_Server
        WHERE Server_ID = p_server_id;
    END delete_authentication_server;
    
    FUNCTION get_authentication_server(p_server_id IN VARCHAR2) RETURN VARCHAR2 IS
        v_ip_address VARCHAR2(20);
    BEGIN
        SELECT IP_address INTO v_ip_address
        FROM Authentication_Server
        WHERE Server_ID = p_server_id;
        
        RETURN v_ip_address;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN 'No data found';
    END get_authentication_server;
       
    -- Процедури для таблиці User_Role
    PROCEDURE insert_user_role(p_user_role IN VARCHAR2, p_server_id IN VARCHAR2) IS
    BEGIN
        INSERT INTO User_Role (User_Role, Server_ID)
        VALUES (p_user_role, p_server_id);
    END insert_user_role;
    
    PROCEDURE update_user_role(p_user_role IN VARCHAR2, p_server_id IN VARCHAR2) IS
    BEGIN
        UPDATE User_Role
        SET Server_id = p_server_id
        WHERE User_Role = p_user_role;
    END update_user_role;
    
    PROCEDURE delete_user_role(p_user_role IN VARCHAR2) IS
    BEGIN
        DELETE FROM User_Role
        WHERE User_Role = p_user_role;
    END delete_user_role;
    
    -- Процедури для таблиці Token_access
    PROCEDURE insert_token_access(p_token_id IN VARCHAR2, p_validity IN INTEGER, p_access_permission IN VARCHAR2, p_server_id IN VARCHAR2) IS
    BEGIN
        INSERT INTO Token_access (Token_ID, Validity, Access_Permission, Server_ID)
        VALUES (p_token_id, p_validity, p_access_permission, p_server_id);
    END insert_token_access;
    
    PROCEDURE update_token_access(p_token_id IN VARCHAR2, p_validity IN INTEGER, p_access_permission IN VARCHAR2, p_server_id IN VARCHAR2) IS
    BEGIN
        UPDATE Token_access
        SET Validity = p_validity, Access_Permission = p_access_permission, Server_ID = p_server_id
        WHERE Token_ID = p_token_id;
    END update_token_access;
    
    PROCEDURE delete_token_access(p_token_id IN VARCHAR2) IS
    BEGIN
        DELETE FROM Token_access
        WHERE Token_ID = p_token_id;
    END delete_token_access;
    
    -- Процедури для таблиці Server_Session
    PROCEDURE insert_server_session(p_session_id IN INTEGER, p_time_start IN INTEGER, p_time_end IN INTEGER, p_token_id IN VARCHAR2, p_server_id IN VARCHAR2) IS
    BEGIN
        INSERT INTO Server_Session (Session_ID, Time_start, Time_end, Token_ID, Server_ID)
        VALUES (p_session_id, p_time_start, p_time_end, p_token_id, p_server_id);
    END insert_server_session;
    
    PROCEDURE delete_server_session(p_session_id IN INTEGER) IS
    BEGIN
        DELETE FROM Server_Session
        WHERE Session_ID = p_session_id;
    END delete_server_session;
    
END auth_server_pkg;
/

CREATE OR REPLACE PACKAGE BODY user_pkg IS

    -- Процедури для таблиці Server_User
    PROCEDURE insert_server_user(p_login IN VARCHAR2, p_email IN VARCHAR2, p_password IN VARCHAR2, p_user_ip IN VARCHAR2, p_server_id IN VARCHAR2) IS
    BEGIN
        INSERT INTO Server_User (Login, Email, Password, User_IP_address, Server_ID)
        VALUES (p_login, p_email, p_password, p_user_ip, p_server_id);
    END insert_server_user;
    
    PROCEDURE update_server_user(p_login IN VARCHAR2, p_email IN VARCHAR2, p_password IN VARCHAR2, p_user_ip IN VARCHAR2, p_server_id IN VARCHAR2) IS
    BEGIN
        UPDATE Server_User
        SET Email = p_email, Password = p_password, User_IP_address = p_user_ip, Server_ID = p_server_id
        WHERE Login = p_login;
    END update_server_user;
    
    PROCEDURE delete_server_user(p_login IN VARCHAR2) IS
    BEGIN
        DELETE FROM Server_User
        WHERE Login = p_login;
    END delete_server_user;
    
    FUNCTION get_server_user(p_login IN VARCHAR2) RETURN VARCHAR2 IS
        v_login VARCHAR2(20);
        v_email VARCHAR2(20);
        v_user_ip VARCHAR2(20);
        v_server_id VARCHAR2(20);
    BEGIN
        SELECT Login,Email,User_IP_address,Server_ID 
        INTO v_login, v_email, v_user_ip, v_server_id
        FROM Server_User
        WHERE Login = p_login;
        
        RETURN 'Login: ' || v_login || ', Email: ' || v_email || ', IP: ' || v_user_ip || ', Server_ID: ' || v_server_id;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            RETURN 'No data found';
    END get_server_user;

END user_pkg;
/

    -- Процедури для таблиці Authentication_Server
BEGIN
    auth_server_pkg.insert_authentication_server('Server1', '10.0.10.1');
END;
BEGIN
    auth_server_pkg.insert_authentication_server('Server2', '10.0.10.2');
END;
BEGIN
    auth_server_pkg.insert_authentication_server('Server3', '10.0.10.3');
END;
DECLARE
    v_result VARCHAR2(100);
BEGIN
    v_result := auth_server_pkg.get_authentication_server('Server3');
    DBMS_OUTPUT.PUT_LINE(v_result);
END;
        /*BEGIN
            auth_server_pkg.update_authentication_server('Server2', '10.0.10.3');
        END;
        DECLARE
            v_result VARCHAR2(100);
        BEGIN
            v_result := auth_server_pkg.get_authentication_server('Server2');
            DBMS_OUTPUT.PUT_LINE(v_result);
        END;
        BEGIN
            auth_server_pkg.delete_authentication_server('Server2');
        END;
        DECLARE
            v_result VARCHAR2(100);
        BEGIN
            v_result := auth_server_pkg.get_authentication_server('Server2');
            DBMS_OUTPUT.PUT_LINE(v_result);
        END;*/

    -- Процедури для таблиці User_Role
BEGIN
    auth_server_pkg.insert_user_role('ADMIN_s1', 'Server1');
END;
BEGIN
    auth_server_pkg.insert_user_role('USER_s1', 'Server1');
END;
BEGIN
    auth_server_pkg.insert_user_role('ADMIN_s2', 'Server2');
END;
BEGIN
    auth_server_pkg.insert_user_role('USER_s2', 'Server2');
END;
        /*BEGIN
            auth_server_pkg.insert_user_role('USER_s3', 'Server2');
        END;
        BEGIN
            auth_server_pkg.update_user_role('USER_s3', 'Server3');
        END;
        BEGIN
            auth_server_pkg.delete_user_role('USER_s3');
        END;
        select * from user_role;*/
        
    -- Процедури для таблиці Token_access
BEGIN
    auth_server_pkg.insert_token_access('token1', 5, 'admin', 'Server1');
END;
BEGIN
    auth_server_pkg.insert_token_access('token2', 5, 'user', 'Server2');
END;
BEGIN
    auth_server_pkg.insert_token_access('token3', 10, 'admin', 'Server2');
END;
/*BEGIN
    auth_server_pkg.update_token_access('token3', 5, 'admin', 'Server3');
END;
BEGIN
    auth_server_pkg.delete_token_access('token3');
END;
select * from token_access;*/

    -- Процедури для таблиці Server_Session
BEGIN
    auth_server_pkg.insert_server_session(1, 0, 5, 'token1', 'Server1');
END;
BEGIN
    auth_server_pkg.insert_server_session(2, 0, 5, 'token2', 'Server2');
END;
BEGIN
    auth_server_pkg.insert_server_session(3, 0, 5, 'token3', 'Server2');
END;
        /*BEGIN
            auth_server_pkg.delete_server_session(3);
        END;
        select * from server_session;*/
        
    -- Процедури для таблиці Server_User
BEGIN
    user_pkg.insert_server_user('Masha', 'Masha@gmail.com', 'Masha123', '192.168.200.2', 'Server1');
END;
BEGIN
    user_pkg.insert_server_user('Misha', 'Misha@gmail.com', 'Misha123', '192.168.200.1', 'Server1');
END;
BEGIN
    user_pkg.insert_server_user('Daniil', 'Daniil@gmail.com', 'Daniil123', '192.168.200.3', 'Server2');
END;
/*BEGIN
    user_pkg.update_server_user('Masha', 'Masha@gmail.com', '87654321', '192.168.200.2', 'Server1'); 
END;
BEGIN
    user_pkg.insert_server_user('Masha1', 'Masha@gmail.com', 'Masha123', '192.168.200.2', 'Server1');
END;
BEGIN
    user_pkg.delete_server_user('Masha1'); 
END;
SELECT * FROM Server_User;*/

DECLARE
    v_result VARCHAR2(100);
BEGIN
    v_result := user_pkg.get_server_user('Daniil');
    DBMS_OUTPUT.PUT_LINE(v_result);
END;








CREATE OR REPLACE TRIGGER check_Authentication_Server
BEFORE INSERT ON Authentication_Server
FOR EACH ROW
BEGIN
    DECLARE
    v_count INTEGER;
    -- Перевірка кількості записів з таким же IP_address
    BEGIN
        SELECT COUNT(*)
        INTO v_count
        FROM Authentication_Server
        WHERE IP_address = :NEW.IP_address;
        
        -- Якщо є хоча б один запис, піднімаємо помилку
        IF v_count > 0 THEN
            RAISE_APPLICATION_ERROR(-20010, 'IP address must be unique.');
        END IF;
    END;
END;
/
BEGIN
    auth_server_pkg.insert_authentication_server('Server6', '10.0.10.1');
END;
CREATE OR REPLACE TRIGGER check_Authentication_Server_del
BEFORE DELETE ON Authentication_Server
FOR EACH ROW
BEGIN
    delete from Server_Session where server_id=:old.server_id;
    delete from Token_access where server_id=:old.server_id;
    delete from User_Role where server_id=:old.server_id;
    delete from Server_User where server_id=:old.server_id;  
END;
/

delete from authentication_server where Server_ID = 'Server2';


CREATE OR REPLACE TRIGGER check_Server_User
BEFORE INSERT OR UPDATE ON Server_User
FOR EACH ROW
BEGIN
    DECLARE
    v_count INTEGER;
    -- Перевірка кількості записів з таким же IP_address
    BEGIN
        SELECT COUNT(*)
        INTO v_count
        FROM Authentication_Server
        WHERE Server_ID = :NEW.Server_ID;
        
        -- Якщо є хоча б один запис, піднімаємо помилку
        IF v_count = 0 THEN
            RAISE_APPLICATION_ERROR(-20011, 'Server_ID not found.');
        END IF;
    END;
END;
/
BEGIN
    user_pkg.update_server_user('Masha', 'Masha@gmail.com', '87654321', '192.168.200.2', 'Server2'); 
END;
BEGIN
    user_pkg.insert_server_user('Daniil', 'Daniil@gmail.com', 'Daniil123', '192.168.200.3', 'Server2');
END;

CREATE OR REPLACE TRIGGER check_User_Role
BEFORE INSERT OR UPDATE ON User_Role
FOR EACH ROW
BEGIN
    DECLARE
    v_count INTEGER;
    -- Перевірка кількості записів з таким же IP_address
    BEGIN
        SELECT COUNT(*)
        INTO v_count
        FROM Authentication_Server
        WHERE Server_ID = :NEW.Server_ID;
        
        -- Якщо є хоча б один запис, піднімаємо помилку
        IF v_count = 0 THEN
            RAISE_APPLICATION_ERROR(-20012, 'Server_ID not found.');
        END IF;
    END;
END;
/

BEGIN
    auth_server_pkg.insert_user_role('USER_s3', 'Server2');
END;
BEGIN
    auth_server_pkg.update_user_role('USER_s1', 'Server2');
END;

CREATE OR REPLACE TRIGGER check_Token_access
BEFORE INSERT OR UPDATE ON Token_access
FOR EACH ROW
BEGIN
    DECLARE
    v_count INTEGER;
    -- Перевірка кількості записів з таким же IP_address
    BEGIN
        SELECT COUNT(*)
        INTO v_count
        FROM Authentication_Server
        WHERE Server_ID = :NEW.Server_ID;
        
        -- Якщо є хоча б один запис, піднімаємо помилку
        IF v_count = 0 THEN
            RAISE_APPLICATION_ERROR(-20013, 'Server_ID not found.');
        END IF;
    END;
END;
/
CREATE OR REPLACE TRIGGER check_Token_access_del
BEFORE DELETE ON Token_access
FOR EACH ROW
BEGIN
    delete from Server_Session where token_id=:old.token_id; 
END;
/
BEGIN
    auth_server_pkg.insert_token_access('token3', 10, 'admin', 'Server3');
END;
BEGIN
    auth_server_pkg.insert_server_session(3, 0, 5, 'token3', 'Server3');
END;
BEGIN
    auth_server_pkg.update_token_access('token1', 5, 'admin', 'Server2');
END;
BEGIN
    auth_server_pkg.delete_token_access('token3');
END;
CREATE OR REPLACE TRIGGER check_Server_Session
BEFORE INSERT ON Server_Session
FOR EACH ROW
BEGIN
    DECLARE
    v_count INTEGER;
    -- Перевірка кількості записів з таким же IP_address
    BEGIN
        SELECT COUNT(*)
        INTO v_count
        FROM Token_access
        WHERE Token_ID=:NEW.Token_ID
        AND Server_ID = :NEW.Server_ID;
        
        -- Якщо є хоча б один запис, піднімаємо помилку
        IF v_count = 0 THEN
            RAISE_APPLICATION_ERROR(-20014, 'Server_ID or Token_ID not found.');
        END IF;
    END;
END;
/
BEGIN
    auth_server_pkg.insert_server_session(1, 0, 5, 'token2', 'Server2');
END;

select * from User_Role
select * from Authentication_Server
select * from Server_User
select * from Token_access
select * from Server_Session

