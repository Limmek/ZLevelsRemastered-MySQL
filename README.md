# ZLevelsRemastered MySQL
 MySQL database add-on for ZLevels Remastered

 This plugin has no usage on its own, it requiers **ZLevels Remastered** and a **MySQL database** to work properly.

 This plugin is used to store data to websites or other backend management systems.

## Configuration

```json
{
  "Options": {
    "saveTimer": 600,
    "updateAtStart": false
  },
  "MySQL": {
    "useMySQL": false,
    "host": "hostname",
    "port": 3306,
    "user": "username",
    "pass": "password",
    "db": "database",
    "table": "zlevelsdata",
    "TurncateDataOnMonthlyWipe": false,
    "TurncateDataOnMapWipe": false
  }
}
```

* **saveTimer** - Save interval, X seconds. (*Default* - **10 minutes**)
* **updateAtStart** - Update tables when plugin is loaded. (*Default* - **false**)
* **TurncateDataOnMonthlyWipe** - Clear database on rust force wipe. (*Default* - **false**)
* **TurncateDataOnMapWipe** - Clear database on map change. (*Default* - **false**)

* **useMySQL** - Enable save to databse. (*Default* - **false**)
* **host** - ip / hostname 
* **port** -  port  (*Default* - **3306**)
* **user** - username
* **pass** - password
* **db** - database
* **table** - table name  (*Default* - **zlevelsdata**)