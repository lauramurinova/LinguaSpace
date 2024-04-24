# Lingua Space

Lingua Space is an XR App to help you learn new languages. Thanks to Meta SDK and its Scene Understanding you can change your living room into a learning space.
Each object has a label, where user can click on and listen to pronounciation of that specific word in the set language. After the user feels confident, they can 
check their pronounciation thanks to the dictation feature. Language can be changed thanks to the menu on left hand, just simply click on the dropdown and change it to the one you desire to learn.

English uses Wit.AI to provide totally free learning experience to everyone.
For more advanced languages, Google API was used, which includes Hungarian, French, Spanish and Bulgarian languages. (Can be easily updated to any proposed language.)

**Core features:** 

- Translations of real life objects with a label above them thanks to scene understanding of Meta SDK.
- User can check its spelling by clicking on dictation and API will take care of speech to text translation to see if your pronounciation is correct.
- If user wants to hear the correct pronounciation, just need to clikc on the label above the object.
- Options menu on the wrist enables the change of the desired language.

**Future:** 

- When Meta comes out with Scene Scripting (was already teased) it will allow real time object recognition and to broaden the translated objects.

## Tech Stack

Meta All in One SDK (Scene Understanding, Passthrough, Anchors, Voice SDK - WIT.AI https://wit.ai) - https://developer.oculus.com/downloads/package/meta-xr-sdk-all-in-one-upm/

Google Text To Speech API - https://cloud.google.com/text-to-speech

Google Translation API - https://cloud.google.com/translate

