# Dictionary Pattern

## Motivation

The idea of this pattern came to me when I first read Robert Martin's book Clean Code. In there it explains why if statements are considered harmful and suggests when there are an awful amount of if statements in our code, it usually indicates the underlying nature of the problem we try to solve is polymorphic.

I am not going to discuss why if statements considered harmful in this post, you can find an excellent post by Jetbrains down below in Reference section. I do see the point as in why excessive amount of if statements are a terrible situation to be in. I don't however, quite agree that polymorphism underpins the problem at hand. Sometimes we could indeed refactor out of this if-inception by polymorphism -- subtype or inclusion polymorphism to be specific. Other times I feel this is too heavy handed because some scenarios are just too simple to warrant the use of subtype polymorphism which is one of the strongest couplings in OO and it is not hard to get wrong! 



## Reference

Code Smells: If Statements[https://blog.jetbrains.com/idea/2017/09/code-smells-if-statements/]

Polymorphism without Inheritence[https://stackoverflow.com/questions/11732422/is-polymorphism-possible-without-inheritance]

On Understanding Types, Data Abstraction, and Polymorphism[http://lucacardelli.name/Papers/OnUnderstanding.A4.pdf]
