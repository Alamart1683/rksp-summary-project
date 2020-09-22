-- Полная версия первого запроса
select IdSt as "ID Студента", count(IdSt) as "Оплаченные семестры", course as "Курс", S.surname as "Фамилия" from Educational_contract E, Student S
where IdST = idStud group by IdST, course, S.surname having course * 2 = count(IdSt)

-- Первый запрос, Фамилии студентов оплатившие все семестры за год
create proc good_student 
as
begin
	select S.surname as "Фамилия" from Educational_contract E, Student S
	where E.IdST = S.idStud group by E.IdST, S.course, S.surname having course * 2 = count(IdSt)
end

go
exec good_student

-- Представление id должника и его специальности
create view loaners 
as
select E.IdST, E.speciality from Educational_contract E, Student S
where E.IdST = S.idStud group by E.IdST, S.course, E.speciality having course * 2 > count(IdSt)

-- Второй запрос реализованный на этом представлении
select speciality, count(speciality) from loaners L
group by speciality

-- Второй запрос, Кол-во не оплативших все семестры для специальностей
create proc bad_student as
begin
	select speciality as "Специальность", count (speciality) as "Должники" 
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

-- Третий запрос, повысить стоимость обучения на указанное число процентов
-- Его реализация выполнена через хранимую процедуру:
create proc price_upd (@speciality nvarchar(50), @procent real)
as
begin
	update price set total = total * (1 + @procent / 100) 
	where speciality = @speciality
end

go
exec price_upd '09.03.04', '150'

-- Четвертый запрос, архивация по отделению
create proc archiving (@department varchar(50))
as
begin
	--Удаление старой таблицы архива если она существует
	if object_id('archive') is not null
		drop table archive
	--Создание новой таблицы архива если она не существует
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
	-- Копирование в неё подоходящих данных
	insert into archive (IdEC, IdST, surname, "group", department, IdPR, speciality, semester_number, total, "date")
	select * from Educational_contract where department = @department
	-- Удалить скопированное из таблицы договоров
	delete from Educational_contract where department = @department
	-- Добавить в таблицу архива столбец даты архивации
	alter table archive add archiving_date date
	-- Добавить текущую дату
	update archive set archiving_date = getdate() 
end

go
exec archiving 'Заочное'

-- Пятый запрос, триггер контроля повышения цены
create trigger price_controller on Price instead of update
as 
begin
	declare @max_price money = '100000'
	declare @speciality varchar(50)
	declare @new_price money
	select @new_price = total from inserted
	select @speciality = speciality from inserted
	if @new_price > @max_price
		raiserror('Превышена максимально допустимая стоимость обучения', 15, 1)
	else
		update Price set total = @new_price where @speciality = speciality
end

-------------------------------------------------------------
------------------ Лабораторная работа 3 --------------------

-- Хранимая процедура удаления пользователя из таблицы users
create proc user_removing(@username varchar(50)) as
begin
	delete from users where username = @username
end

go
exec user_removing 'student2'

-- Хранимая процедура добавления пользователя в таблицу users
create proc user_creating(@username varchar(50), @password varchar(20), @type varchar(20)) as
begin
	insert into users values(@username, @password, @type)
end

go
exec user_creating 'newadmin', '77887788', 'admin'


