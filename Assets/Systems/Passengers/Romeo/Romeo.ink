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

Mrrow # emotion:default
    + [! ! !]
        
        -> p1
    + [? ? ?]

        -> p1
    + [...]

        -> p1

= p1
rrrr rrrr
    + [Hey there.]
        
        -> p2
    + [A collar?]

        -> p2
    + [Hm.]

        -> p2

= p2
rrrya # name_revealed
    + [This address—]
        
        -> p3
    + [<b>25 Harmony Hill Drive</b>?]

        -> p3
    + [...]

        -> p3
        
= p3
 # end_greeting

Mrrow! # requests_start
    + [Alright, alright. I'm driving.]
        
        -> p4
    + [You've got somewhere to be, hm?]

        -> p4
    + [...]

        -> p4
        
= p4
raow
    + [Well, you're far from the weirdest passenger I've had today.]
        
        -> p5
    + [Not going to pay the fare, are you?]

        -> p5
    + [...]

        -> p5
        
= p5
rrrya
    + [You're lucky you're cute, or I'd have left you at the stop.]
        
        -> p6
    + [Why am I even driving a cat around, anyway?]

        -> p6
    + [...]

        -> p6
        
= p6
nrrr nrrr
    + [I feel like I've seen these houses before.]
        
        -> p7
    + [Do you ever feel like you're going in circles?]

        -> p7
    + [...]

        -> p7
        
= p7
...
    + [It's not like you can understand me.]
        
        -> p8
    + [Why am I even telling you this?]

        -> p8
    + [...]

        -> p8
        
= p8
Mrrow!
    + [Oh, so you were listening?]
        
        -> p9
    + [She would have liked you.]

        -> p9
    + [...]

        -> p9
        
= p9
nrr
    + [She always said cats were good listeners.]
        
        -> p10
    + [Did she send you here or something?]

        -> p10
    + [Hah...]

        -> p10
        
= p10
...
    + [What am I doing...I'm talking to a cat like it's her.]
        
        -> p11
    + [You're not just some cat, are you?]

        -> p11
    + [...]

        -> p11
        
= p11
rryow # spawn_dest

rrya # dropoff # requests_end
    + [Maybe you're just as lost as I am.]
        
        -> p12
    + [...]

        -> p12
        
= p12

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