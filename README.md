# Инструкция по деплою RusGuardInt сервиса

## 1.Удостовериться в наличии IIS сервера

Проверить установлен ли IIS сервер на машине,где будет развернут данный сервис.
Если сервер не установлен перейти в Панель управления->Программы->Включение или отключение компонентов Windows->Службы IIS

![](https://professorweb.ru/my/ASP_NET/sites/level3/files/img51068.jpg)

Так же выбрать пункты .NET Framework 3.5 -> Активация WCF по HTTP и 

.NET Framework 4.8 -> Службы WCF -> Активация по HTTP и 

ASP .NET 4.8

# 2.Развернуть сервис RusGuardInt

#### 1.Открыть Диспетчер служб IIS.

#### 2. Щелкнуть правой кнопкой на вкладку сайты ->Добавить веб-сайт.

#### 3. Указать путь к распакованному сервису,сервис положить в корневую папку. 

   Указать нужный IP-адрес и порт

#### 4.Перейти во вкладку пулы приложений -> Дополнительные парметры

#### 5. Во вкладке "Разрешены 32-разрядные приложения" установить True

#### 6. Во вкладке "Режим запуска" установить AlwaysRunning

#### 7.Перейти в расположение программы файл Web.config -> в строке ConnStr в поле  value указать адрес расположения сервиса RusGuard
#### 8.Перейти в расположение программы файл Web.config -> в строке Login в поле  value указать логин администратора
#### 9.Перейти в расположение программы файл Web.config -> в строке Pass в поле  value указать пароль администратора 
 
