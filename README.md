# NBP Application

## General info
Application downloads currency rates from NBP API once per day and save them in database.
Based on data, user can:
* check last currency rate
* check historical currency rate
* recalculate PLN amount to chosen currency and vice versa (based on current rates)
* calculate converter between two chosen currencies
* get CSV file with historical currency rates
	
## Technologies
Project is created with:
* ASP.NET Core 5.0
* SQLite
* Entity Framework Core 5.0
* Json.NET 13.0
* Swagger

## Examples of use

### All controllers:
![User controllers](https://github.com/SzymonTomala/NBPApplication/blob/main/controllers.PNG)

### Getting CSV with historical THB rates, from 18.02.2022 to 20.02.2022:
![CSV with historical currency rates](https://github.com/SzymonTomala/NBPApplication/blob/main/example.PNG)
