# Design Notes

This file keeps some thoughts I had throughout writing this little app. 

## Delete Sounds vs Keeping Sounds 

It would be possible to read the soundfiles from last time into the memory on startup. 
This might be interesting, however I decided its nicer for the user to have a cleaned-up environment. 
I think some audios can become pretty big if it is never cleaned up, so cleaning should be the default behaviour. 

## Delete Sounds on Startup vs on Close

I decided to clean the sounds folder on Startup, reason for this is 
that you can then look through your files and maybe save them after closing the app. 
As I wanted to use this for memes mostly, and I am rather cheap, 
I thought maybe something looks funny in retrospect and is for saving. 

Deleting it on Close would have the benefit that your system is the cleanest, 
but ... well a little messy is ok I guess. 

## AWS SDK instead of HTTP Call 

It would be possible to get the polly-responses using *normal* https. 

There are two reasons why I did go with the SDK: 

1) The API might get updates, which should be implemented in the SDK making a Change easier (version bumping and then check compilation)
2) For HTTP I would have to implement the Auth stuff with generating tokens, which is a bit of an overhead.

While the SDK documentation is not the very very best, it turned out to be ok. 
If you change / add more AWS criteria and have to google for things, 
just make sure that you first look for auth and the credentials, 
as the PollyClient is one of many Clients that need the Auth Process.

