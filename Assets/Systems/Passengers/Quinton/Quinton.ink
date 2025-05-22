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

Haha! Glad I saw you! My legs were starting to hurt. # name:??? # emotion:default # voice:1

Take me to 1000… No 1100 Flower Blvd. # end_greeting # voice:2

Haha! I'm actually gonna be on time at this rate. # requests_start  #emotion:smile # voice:3

Everyone's gonna be surprised! # pause:4 # voice:4

Hey, Driver... # emotion:disappointed # voice:5

I know this looks bad, but... # voice:6

I just realized I don't have my wallet on me. # voice:7

If you give me a ride, next time I'll bring enough to pay you back and then some. Promise. # emotion:default # voice:8
    + [No.]
        C'mon! I’ll even leave my hula girl with you. She's probably worth a lot. # gift:0 # voice:9
        ** [I'm dropping you off here.]
            Wait! # emotion:disappointed # voice:10
            
            My hula girl! # voice:11
            
            Bro...! # requests_end # kick_out # voice:12
            
            -> DONE
        ** [Fine.]
            Oh man, thank you! # emotion:smile # mood:10 # voice:13
            
            -> p1
    + [Alright.]
        Oh man, thank you! Here, I’ll give you my precious hula girl as collateral that I’ll be back again. # emotion:smile # mood:10 # gift:0 # voice:14
        
        -> p1

= p1
You're a lifesaver! # emotion:smile # pause:2.5 # voice:15

I can tell you're a good guy, since you let me stay. # voice:16

This town could use more good people. # voice:17

Uh... hey driver, ever fallen in <glitch>love</glitch>? # voice:18
    + [Once.]
        What kinda girl was she? # voice:19
        ** [Hardworking]
            Sounds like my girl! # mood:10 # voice:20
            
            I try to visit her at work but she won't let me stay long. Says I distract her. # emotion:smile # voice:21
            
            I get it. I mean, who wouldn't be distracted by all this? # emotion:laugh # voice:100
            
            I bet she's happy to see me, though. # emotion:smile # voice:22
            
            Whenever I close my eyes I can see her smiling. # voice:23
            
            She's given me more chances than I deserve. # emotion:default # voice:24
            
            -> p2
        ** [Talkative]
            Sounds like my girl! # mood:10 # voice:22
            
            A girl that speaks her mind is easier to deal with for sure. # voice:26
            
            When my ex and I fought she would ignore me for days. # emotion:laugh # voice:27
            
            -> p2
        ** [Fleeting]
            …What does that even mean??? # emotion:disappointed # voice:28
            
            -> p2
            
    + [...]
        Hah, well...<glitch>love</glitch> might be a strong word, but... # voice:29
        
        Whenever I close my eyes I can see her smiling. # voice:23
        
        She's given me more chances than I deserve. # emotion:default # voice:24
        
        -> p2

= p2
I just don't wanna mess this one up. # emotion:default # voice:32

I remember when I first got to talkin' with her a few months ago. # emotion:smile # voice:33

She used to work at the diner my uncle runs. # voice:34

My uncle's an asshole and one time she snapped at him. Someone needed to get him off his horse. # pre_emotion:laugh # pre_emotion:laugh # voice:35

Tall...horse? Something like that. # emotion:default # voice:36

Ah, she would know. # emotion:smile # voice:37

She's smart. # voice:38

Smart <i>and</i> strong...I could learn a lot from her. # voice:39

Ah...sorry, I drank a bit before this. # pause:1.5 # voice:40

Didn't mean to get sappy with you. # emotion:default # voice:41

I've just been down on my luck recently. Drinkin' helps. # voice:42

Speakin' of, do you want some? # emotion:smile # voice:43

Ah shit what am I sayin'! You're driving! Haha, that could've been bad. # emotion:laugh # pause:1.5 # voice:44

Man... it just feels like things need to slow down. # emotion:default # voice:45

My Grandpa's age has been catchin' up to him. # voice:46

That stubborn old man actually lets me help him now. # emotion:smile # voice:47

We used to get into these loud arguments, but it's been real quiet lately. # emotion:default # voice:48

It's weird. You'd think I'd like having peace in the house, but it just reminds me that he won't be here forever. # emotion:disappointed # voice:49

There's never enough time... # voice:50

Or money! I could always use more of that. # emotion:smile # voice:51

Though, once I finally do get rich, I'll get a new car that actually works. An '06 Pluto, bright red. # voice:52

And I'll leave this town with my girl. # emotion:smile # voice:53

...I'll probably leave Grandpa something, too. Least I could do. # pause:3 # voice:54

Thanks again for letting me stay earlier. I swear I have the money to pay for this ride, just not on me. # emotion:default # voice:55

Speakin' of money, Driver, give me some lotto numbers! # emotion:smile # voice:56
    + [2 5 11 24 25]
        Haha, thanks, dude! # mood:10 # voice:57

        -> p3
        
    + [7 9 13 37 49]
        Haha, thanks, dude! # mood:10 # voice:57

        -> p3
            
    + [I'd rather not.]
        What? You think I won’t win? # emotion:disappointed # mood:-10 # voice:59
        
        -> p3

= p3
I usually just choose the same numbers over and over. Maybe that's why I haven't won yet. # emotion:default # voice:60

I'll hit it big soon, though. # pause:3.5 # voice:61

// play cell phone audio
Hey, it's Q-dog! # name:Q-dog # emotion:phone # voice:62

Ah, I mean it's Quinton. I'm Quinton. # name:Quinton # voice:63

Oh, from SmallMart! # voice:64

Yeah... # voice:65

...

Other candidates?! # voice:66

Who else applied to work at this shithole?! I'm perfectly capable! # voice:67

...

THAT'S why? # voice:68

That happened years ago, who even cares— # voice:69

Bastard hung up on me. # emotion:default # voice:70

... # emotion:disappointed

Man. This blows. I've applied to a ton of different places and I've got nothin' to show for it. # voice:71

They don't want me because of some shit I didn't even do. # emotion:default # voice:72

Hey Driver, here's some advice: don't be like me. # voice:73

If somethin' seems too good to be true, it is. Money ain't free. # voice:74

I got in some deep shit with a few sharks a while back. # spawn_dest # voice:75

Long story short, I ended up takin' the fall for someone who I thought was a friend in order to clear my debt. Spent some time behind bars. # voice:76

Honestly, I don't even know what the court was sayin' I did. # voice:77

Thinkin' about it pisses me off to no end. # voice:78

Reminds me that the real bastards who did it walked away scot-free. # emotion:disappointed # voice:79

Some world we live in. # emotion:default # voice:80

... # dropoff # requests_end
 
~ temp mood = GetMood()

{
    - mood > 150:
        Hey, thanks again for listening to me earlier. # emotion:smile # voice:81
        
        You're easy to talk to. # voice:82
        
        You might see more of me, I probably won't be getting a new car soon. # voice:83
        
        Riding in here was a nice break from all the stuff I gotta deal with. # voice:84
        
    - mood > 50:
        Thanks, this was much better than walking. # emotion:smile # voice:85

        Drive safe, dude. # voice:86
        
    - else:
        Looks like we're here. # voice:87
        
        Later. # voice:88
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