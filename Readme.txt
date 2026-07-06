1. Lai palaistu datubāzi:
cd database
docker-compose up

2. Palaist backendu
If using VS code
cd ..
cd backend
dotnet run

3. Palaist frontendu
cd ..
cd frontend
npm run dev

Izvēlēto tehnoloģiju pamatojums:

1) Datu slānī - Postgressql
PostgreSQL ir nozares standarts konteinerizētās izstrādes vidēs. Salīdzinot ar citām smagnējākām relāciju datubāzēm
(piemēram, MS SQL Server), PostgreSQL Docker attēls (image) ir ievērojami vieglāks, patērē mazāk operatīvās atmiņas (RAM) un startējas dažu
sekunžu laikā. Tas nodrošina, ka visa sistēma Docker vidē darbojas ātri, stabili un ir viegli darbināma uz jebkura lokālā datora.

2) Backend slānī - C#
C# tika izvēlēts galvenokārt tāpēc, ka tajā ir pieejams Entity Framework Core (EF Core) — viens no jaudīgākajiem un
nobriedušākajiem ORM (Object-Relational Mapping) rīkiem nozarē.

3) Frontend slāni - Vue.js
Vue.js tika izvēlēts tā jaudīgās un intuitīvās reaktivitātes sistēmas dēļ
