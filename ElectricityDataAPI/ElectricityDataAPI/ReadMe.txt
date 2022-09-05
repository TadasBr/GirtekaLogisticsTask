Azure does not allow to give permission to all Ips I deployed app can find swagger UI here:
https://electricitydataapi.azurewebsites.net/swagger/index.html

If you want to test Data seeding yourself, im pretty sure all u have to do is change ConnectionStrings in appsettings.json
it takes approximately 15minutes.

Also I know that it should update database once data is updated, can do that by simply saving Updated DateTime to file and then
just check on run everytime with it.

Main thing about my solution is that I created 2 tables 1 For RealEstate (PK objectNumber) and rest of columns doesnt change i checked
myself in excel and 1 for ElectricityReport where changing data is stored. SO this increase data storing efficiency by a lot since
there's about 700k rows to be stored.

