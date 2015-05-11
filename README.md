[![Build status](https://ci.appveyor.com/api/projects/status/github/NaosFramework/Naos.FileJanitor?branch=master&svg=true)](https://ci.appveyor.com/project/NaosLLC/naos-filejanitor)

Naos.FileJanitor
================
A clean up utility to remove old processing files.  It was created to age off ETL intermediary files.

```none
Usage
-----
cleanup|cleanup|c: Removes old files
	/dateretrievalstrategy  : The date retrieval strategy to use. (DateRetrievalStrategy (CreateDate/LastUpdateDate/LastAccessDate)) (Default = LastUpdateDate) 
	/deleteemptydirectories : Whether or not to delete directories that are or become empty during cleanup. (Default = False) 
	/recursive              : Whether or not to evaluate files recursively on the path. (Default = True) 
	/retentionwindow        : The time to retain files (in format dd:hh:mm). (String) (Required) 
	/rootpath               : The root path to evaluate (must be a directory). (String) (Required) 

```