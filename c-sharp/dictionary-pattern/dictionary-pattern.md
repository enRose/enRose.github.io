# Rest API & Dictionary Pattern

Before we delve into this pattern, let's understand some background as in why and how this pattern come to existence.

## The Single Page Application

I remember back in around 2011 there was this new way of building websites call SPA started to gaine significant amount of attention among web community. With SPA we give the website everything it needs from the get-go so there is no round-trip between browser and server, website has enough state that to satisfy user interaction, offline interactivity and responsiveness.

The result is a much pleasant user experience as there is no lagging. However, the biggest motivation in my opinion is to off-load as much as computation to the client as possible therefore cut down the cost of running in the cloud.

As the rise of Javascript frameworks such as 2009 AngularJs[https://en.wikipedia.org/wiki/AngularJS] and shortly followed by 2013 ReactJs[https://en.wikipedia.org/wiki/React_(web_framework)] among web community. New Javascript frameworks had made development in SPA relatively easy and this SPA approach had become the defacto standard of how web apps should be built and it worked really well for a number of years.   

## Progressive Web App

Progressive Web App got its name out by an article published in 2015 by Alex Russell. I won't go into details as what attributes necessitate a PWA but in a nutshell, as modern web browser technology advances, more and more capabilities are supported in native browser APIs that make web sites feel and act 'app-y' such as can be added to mobile device's home screen, push notification badge, etc. without some of the constraints in ios or Android native apps. 

They might be built in Angular or Reactjs, PWAs however, is not the same as SPAs. 


## Progressive loading


## Reference

Single Page Application[https://en.wikipedia.org/wiki/Single-page_application]

Choose between SPA vs traditional web app[https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/choose-between-traditional-web-and-single-page-apps]

Progressive Web App[https://medium.com/@amberleyjohanna/seriously-though-what-is-a-progressive-web-app-56130600a093]

Progressive loanding[https://developer.mozilla.org/en-US/docs/Web/Progressive_web_apps/Loading]
