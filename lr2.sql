-- ������ ������ ������� �������
select IdSt as "ID ��������", count(IdSt) as "���������� ��������", course as "����", S.surname as "�������" from Educational_contract E, Student S
where IdST = idStud group by IdST, course, S.surname having course * 2 = count(IdSt)

-- ������ ������, ������� ��������� ���������� ��� �������� �� ���
create proc good_student 
as
begin
	select S.surname as "�������" from Educational_contract E, Student S
	where E.IdST = S.idStud group by E.IdST, S.course, S.surname having course * 2 = count(IdSt)
end

go
exec good_student

-- ������������� id �������� � ��� �������������
create view loaners 
as
select E.IdST, E.speciality from Educational_contract E, Student S
where E.IdST = S.idStud group by E.IdST, S.course, E.speciality having course * 2 > count(IdSt)

-- ������ ������ ������������� �� ���� �������������
select speciality, count(speciality) from loaners L
group by speciality

-- ������ ������, ���-�� �� ���������� ��� �������� ��� ��������������
create proc bad_student as
begin
	select speciality as "�������������", count (speciality) as "��������" 
	from Educational_contract
	where IdST in (
		select E.IdST from Educational_contract E, Student S
		where E.IdST = S.idStud 
		group by E.IdST, S.course, E.speciality 
		having course * 2 > count(IdSt)
	)
	group by speciality 
end

go
exec bad_student

-- ������ ������, �������� ��������� �������� �� ��������� ����� ���������
-- ��� ���������� ��������� ����� �������� ���������:
create proc price_upd (@speciality nvarchar(50), @procent real)
as
begin
	update price set total = total * (1 + @procent / 100) 
	where speciality = @speciality
end

go
exec price_upd '09.03.04', '150'

-- ��������� ������, ��������� �� ���������
create proc archiving (@department varchar(50))
as
begin
	--�������� ������ ������� ������ ���� ��� ����������
	if object_id('archive') is not null
		drop table archive
	--�������� ����� ������� ������ ���� ��� �� ����������
	--if object_id('archive') is null
	create table archive (
		IdEC int NOT NULL,
		IdST int NOT NULL,
		surname nvarchar(50) NOT NULL,
		"group" nvarchar(50) NOT NULL,
		department nvarchar(50) NOT NULL,
		IdPR int NOT NULL,
		speciality nvarchar(50) NOT NULL,
		semester_number int NOT NULL,
		total money NOT NULL,
		"date" date NOT NULL, )
	-- ����������� � �� ����������� ������
	insert into archive (IdEC, IdST, surname, "group", department, IdPR, speciality, semester_number, total, "date")
	select * from Educational_contract where department = @department
	-- ������� ������������� �� ������� ���������
	delete from Educational_contract where department = @department
	-- �������� � ������� ������ ������� ���� ���������
	alter table archive add archiving_date date
	-- �������� ������� ����
	update archive set archiving_date = getdate() 
end

go
exec archiving '�������'

-- ����� ������, ������� �������� ��������� ����
create trigger price_controller on Price instead of update
as 
begin
	declare @max_price money = '100000'
	declare @speciality varchar(50)
	declare @new_price money
	select @new_price = total from inserted
	select @speciality = speciality from inserted
	if @new_price > @max_price
		raiserror('��������� ����������� ���������� ��������� ��������', 15, 1)
	else
		update Price set total = @new_price where @speciality = speciality
end

-------------------------------------------------------------
------------------ ������������ ������ 3 --------------------

-- �������� ��������� �������� ������������ �� ������� users
create proc user_removing(@username varchar(50)) as
begin
	delete from users where username = @username
end

go
exec user_removing 'student2'

-- �������� ��������� ���������� ������������ � ������� users
create proc user_creating(@username varchar(50), @password varchar(20), @type varchar(20)) as
begin
	insert into users values(@username, @password, @type)
end

go
exec user_creating 'newadmin', '77887788', 'admin'


