# Unitypackage-Extract-and-Clean
Unitypackage Extract and Clean is a Program to get files out of a .unitypackage without importing them into unity and optionally delete all .dll and .cs files in the exported package folder.

This program is a fork created from EasyExtractUnitypackage which is originally made by HakuSystems, but this one includes a neat feature of deleting .dll and .cs files from it.

## Why would I want to delete .dll and .cs files from it?
Well the main reason would be for safety porpuses. As you may or may not know, Unity can be quite dangerous if you're importing scripts from people or sources you don't know.
These can contain things such as loggers and other malicous things. Maybe you've imported such thing before and didn't notice and suddenly you've lost something you've once owned such as an account!

You always have the choice to NOT delete the files if you wanna use it's scripts and tools later (if you trust that it's safe)!

## Can I not just scan the .unitypackage with my antivirus and call it a day?
Simple answer, no... 99% of the time, your antivirus will report that there are no problems with it. Believe me, I've tested it with files I knew are not safe.

# Setup and Requirements:
Other than basic .net things that you likely already have, nothing! 
Once you download the exe from the [releases](https://github.com/Jan-Fcloud/Unitypackage-Extract-and-Clean/releases/) tab, you're good to go!

## P.S:
I'm no C# expert but from the testing I've done and whatnot, it should not break! Wow!

*(But tell me if I managed to mess it up somehow...)*
