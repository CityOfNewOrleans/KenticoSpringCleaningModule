# Kentico Spring Cleaning Module

This module does two things:

* Moves all attachments from the database to the file system, clearing the binary data out of the db as it goes.
* Removes all attachment history, permanently destroying the attachment binaries.

All you need to do is install this module in a Kentico 8.2 instance, then a single click will accomplish either of those two things.

## Installing

In the Kentico Admin interface:

1. Go to the `Modules` module.
1. Click `New Module`.
1. Name it "Spring Cleaning" and give it the code name `Spring Cleaning`
1. Click `Save`.
1. On the side menu that appears, go to `Sites` and assign the module to any sites you want it to appear in the admin interface for. _It needs to be assigned to at least one site or it will be inaccessible._
1. Go to `User Interface`. On the `CMS` tree navigator on the side, expand `Administration` > `Custom`.
1. Select `Custom` and hit the `+` button. Fill out the following values:

    * Display name: SpringCleaning
    * Code name: SpringCleaning
    * Element Content Type: URL
    * Element Target URL: ~\CMSModules\SpringCleaning\Default.aspx

1. Click Save.
1. In File Explorer, copy the `App-Code`, `CMS_Modules`, and `CMS_Scripts` folders from this repo to the `CMS` folder in you Kentico instance.
1. Go to the `System` module and restart the site.

After all that, the `Spring Cleaning` module should appear in the Kentico Admin menu for the sites you assigned it to.

## Using the Module

Couldn't be simpler. For each of the tasks, just hit `Start`. To stop, `Hit` stop. I would however, recommend reloading page in between running either task.