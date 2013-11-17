initialround
============

Web app for interviewing technical candidates in the browser

Distributed under the terms of the BSD 3-clause License. See the LICENSE file for more information.

## Running

This code is intended for educational purposes.

If you would like to run this code, it should be possible to get this running on Windows Azure fairly simply.

You will need to take the following steps:

- Create a SQL Azure database and storage account
- Run Setup.sql to create tables
- Run the setup application to create an admin user
- Update the deployment settings to include the new connection strings
- Create new encryption keys and place them in the Keys settings file
- Setup email by entering SMTP server info in the Settings file
- Include dependencies from NuGet which have not been included here
- Include some Javascript dependencies which have not been included here
- Deploy the web and email worker roles to Windows Azure instances
