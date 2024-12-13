Hello hello! #speaker:Adamastor #portrait:adamastor #layout:right 
-> main

=== main ===
How are you feeling? #playEvent:turnOff
+ [Happy] #checkAnswer:true
    That makes me feel happy as well! 
+ [Sad] #checkAnswer:false
    Oh, well that makes me sad too.
    - Do you want to try to make him happy? #speaker:Narrador #portrait:narrador #layout:right
        + [Yes]
            Then, try to complete this poem!
            ...    #playEvent:startPoem
            -> END 
        + [No]
            Okay, I am sorry!

- Don't trust him! #speaker:Tu #portrait:player #layout:left #playEvent:turnOn

Well, do you have any more questions? #speaker:Adamastor #portrait:adamastor #layout:right
+ [Yes]
    -> main
+ [No]
    Goodbye then!
    -> END