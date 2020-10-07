# About
This is the last contact manager you will ever need. As long as the only thing you need is email addresses. Features include Adding, Editing, Removing and Importing contacts.

# Configuration
By default, the app will automatically build and seed a database to your (localdb) if it is available.

To change the sql database connection string, open appsettings.config and change the "ConnectionStrings:Local" value to your database connection string.

# Imports
You can import by using the "Import" button and providing a comma delimited csv file with columns:

    "FirstName","LastName","Email","EmailType"

Current limitations prevent you from importing more than one email per contact.

A sample import is provided with the project. "import.csv"

# Bugs
Hangfire does not create tables upon first run with initial migrations. Simply start the app a second time to work around this bug.
