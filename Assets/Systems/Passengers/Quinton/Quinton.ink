EXTERNAL GetRideNumber()
EXTERNAL UpdateCurrentKnot(knotName)
EXTERNAL PostDropoff()
EXTERNAL GetMood()

/*
VALID TAG GUIDE (tags are used like this: "Hey guys!! # tagName:tagValue" )
----------------------------------------------------------
- "emotion:x"
    - Use to specify a passenger's expression (e.g. Hi! # emotion:default)
    - This tag does NOT need to be specified every line—lines that do not have an emotion tag will use the previous expression
- "pre_emotion:x"
    - Use to specify an expression to play BEFORE the line is said (e.g. This will play after! # pre_emotion:default)
    - This tag DOES need to be specified every line—previous pre-expressions will not carry over to future lines
- "name_revealed"
    - Use to reveal a passenger's name on this line (e.g. My name is Maxine! # name_revealed)
    - This tag does NOT need to be specified every line—use it once, and from then on the character's name is revealed
- "pause:x"
    - Use to pause a certain amount of time before saying the line (e.g. This is said after 2 seconds! # pause:2)
    - Does take non-integer numbers (e.g. 1.75, 20.3)
- "spawn_dest"
    - Use to spawn the current destination at a specific line (e.g. Spawn my house! # spawn_dest)
    - MUST BE PRESENT IN A CHARACTER'S RIDE TO FINISH THE RIDE
    - DO NOT USE MORE THAN ONCE PER RIDE
- "gift:x"
    - Use with the gift's name to spawn a gift item at a specified line (e.g. Here's a duck! # gift:duck)
- "hallucinating:[on/off]"
    - Use to specify whether a line should induce hallucination VFX or not (e.g. I'm going to pass this class!! # hallucinating:on)
    - This tag DOES NOT need to be specified every line, instead, trigger it with on/off calls
- "kick_out"
    - Use to specify whether a passenger is kicked out at this line (e.g. SCREW YOU!! # kick_out)
    - The line that has this tag will be the last line said before the passenger is kicked out
- "time_loop"
    - Use to trigger a time loop effect (e.g. Time loop starts here! # time_loop)
    - THERE SHOULD BE NO MORE LINES AFTER THIS TAG
- "vox:x"
    - Use to specify a voice line to play for this line (e.g. Hear my beautiful voice # vox:1)
- "end_greeting"
    - Use to specify the END of the initial greeting once a passenger gets in the car (e.g. Take me to McDonalds! # end_greeting)
- "dropoff"
    - Use to specify the START of the dropoff salute (e.g. Well this is me! # dropoff)
    - The line this tag is attached to should be the START of the salute
- "requests_start"
    - Use to specify when dashboard requests should be enabled (e.g. Start now! # requests_start)
- "requests_end"
    - Use to specify when dashboard requests should be disabled (e.g. Bye bye! # requests_end)
- "mood"
    - Use to specify how much a line should affect mood (e.g. This subtracts! # mood:-10)
    - Negatives are accepted by placing "-" in front of the number, positives simply have no symbol
*/

// ALWAYS STARTS HERE
-> start

=== start ===
~ UpdateCurrentKnot("start") // KNOT NAME MUST BE SET UNDER EVERY KNOT

// Get the current ride number for this passenger
~ temp ride = GetRideNumber()
-> ride

=== _1 ===
~ UpdateCurrentKnot("_1")

Haha! Glad I saw you! My legs were starting to hurt. # emotion:default

Take me to 1000… No 1100 Flower Blvd. # end_greeting

I’m crazy late, can you drive faster? Damn… # requests_start # pause:2

Hey, Driver... # emotion:disappointed

I just realized I don't have my wallet on me.

If you give me a ride, next time I'll bring enough to pay you back and then some. Promise. # emotion:default
    + [No.]
        C'mon! I’ll leave my hula girl with you. Probably worth a lot. # gift:0
        ** [I'm dropping you off here.]
            Wait! # emotion:disappointed
            
            My hula girl!
            
            Bro...! # requests_end # kick_out
            
            -> DONE
        ** [Fine.]
            Oh man, thank you! # emotion:smile # mood:10
            
            -> p1
    + [Alright.]
        Oh man, thank you! # emotion:smile # mood:10
        
        Here, I’ll give you my precious hula girl as a promise that I’ll be back next time. # gift:0
        
        -> p1

= p1
You're a lifesaver! # emotion:smile # pause:2.5

Man...there's this girl who's got me all nervous.

Hey driver, ever fallen in <glitch>love</glitch>?
    + [Once.]
        What kinda girl was she?
        ** [Hardworking]
            Sounds like my girl! # mood:10
            
            I try to visit her at work but she won't let me stay long. Says I distract her. # emotion:laugh
            
            I bet she's happy to see me, though. # emotion:smile
            
            Whenever I close my eyes I can see her smiling.
            
            She's given me more chances than I deserve. # emotion:default
            
            -> p2
        ** [Talkative]
            Sounds like my girl! # mood:10
            
            A girl that speaks her mind is easier to deal with for sure.
            
            When my ex and I would fight she would ignore me for days. # emotion:laugh
            
            -> p2
        ** [Fleeting]
            …What does that even mean??? # emotion:disappointed
            
            -> p2
            
    + [...]
        Hah, well...<glitch>love</glitch> might be a strong word, but...
        
        Whenever I close my eyes I can see her smiling.
        
        She's given me more chances than I deserve. # emotion:default
        
        -> p2

= p2
I just don't wanna mess this one up. # emotion:default

I remember when I first got to talkin' with her a few months ago.

She used to work at the diner my uncle runs.

My uncle's an asshole and one time she snapped at him.

Someone needed to get him off his horse. # emotion:smile # pre_emotion:laugh

Tall...horse? Something like that. # emotion:default

Ah, she would know. # emotion:smile

Smart <i>and</i> strong...I could learn a lot from her. # pause:1.5

Ah...sorry, I drank a bit before this.

Didn't mean to get sappy with you. # emotion:default

I've just been down on my luck recently. Drinkin' helps.

Got kicked out of my old place and moved in with my grandpa. Still gettin' used to it.

I always get into these loud arguments with him.

...Hate to admit it, but he's probably right. Pisses me off.

For some reason, everyone says I'm just like him, but I don't see it.

Once I get rich, I'll get a new car that actually works. An '06 Pluto, bright red.

And I'll leave this town with my girl. # emotion:smile

...I'll probably leave Grandpa something, too. Least I could do. # pause:3

Thanks again for letting me stay earlier. I swear I have the money to pay for this ride, just not on me. # emotion:default

Speakin' of money, Driver, give me some lotto numbers! # emotion:smile
    + [2 5 11 24 25]
        Haha, thanks, dude! # mood:10

        -> p3
            
    + [I'd rather not.]
        What? You think I won’t win? # emotion:disappointed # mood:-10
        
        -> p3

= p3
I usually just choose the same numbers over and over. # emotion:default

Maybe that's why I haven't won yet.

I'll hit it big soon, though. # pause:3.5

// play cell phone audio
Hey, it's Q-dog! # emotion:phone

Ah, I mean I'm Quinton. It's Quinton.

Oh, from SmallMart!

Yeah...

...

Other candidates?!

Who else applied to work at this shithole?! I'm perfectly capable!

...

THAT'S why?

That happened years ago, who even cares—

Bastard hung up on me. # emotion:default

... # emotion:disappointed

Man. This blows.

I've applied to a ton of different places and I've got nothin' to show for it.

They don't want me because of some shit I didn't even do. # emotion:default

Hey Driver, here's some advice: don't be like me.

If somethin' seems too good to be true, it is. Money ain't free.

I got in some deep shit with a few sharks a while back. # spawn_dest

Long story short, I ended up takin' the fall for someone I thought was a friend in order to clear my debt.

Spent some time behind bars.

Honestly, I don't even know what the court was sayin' I did.

Thinkin' about it pisses me off to no end.

Reminds me that the real bastards who did it walked away scot-free. # emotion:disappointed

Some world we live in. # emotion:default

... # dropoff # requests_end
 
~ temp mood = GetMood()

{
    - mood > 150:
        Hey, thanks again for listening to me earlier. # emotion:smile
        
        You're easy to talk to.
        
        You might see more of me, I probably won't be getting a new car soon.
        
        Riding in here was a nice break from all the stuff I gotta deal with.
        
    - mood > 50:
        Thanks, this was much better than walking. # emotion:smile

        Drive safe, dude.
        
        I'll see you around!
    - else:
        Looks like we're here.
        
        Later.
}

~ PostDropoff() // Drops passenger off at destination
-> DONE

=== _2 ===
~ UpdateCurrentKnot("_2")

this is two! # larie:default

this shouldn't change!

second line in two! # fallow:default # name_revealed
-> DONE

=== _3 ===
~ UpdateCurrentKnot("_3")

heyyyy what's up tyhree! # larie:default

this shouldn't change!

second line in three! # fallow:default # name_revealed
-> DONE









-> END