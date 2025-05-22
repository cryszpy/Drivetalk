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
- "name:x"
    - Use to change a passenger's name
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

Yo! What's up Taxi-man? # name:??? # emotion:default # voice:1

You got time for a ride to Sycamore park? # voice:2

I appreciate it! # end_greeting # voice:3

Oh. Y'know... # pre_emotion:fluff # pause:5 # requests_start # voice:4

When I heard a taxi driver was coming to town, I was stoked. # voice:5

'Cause dude. Don't get me wrong—it's a small town, but walking takes # voice:6

F O R E V E R. # voice:7

So I'm glad you're here, man. # voice:8
    + [Thanks.]
        Of course, man. But I'm curious... # pre_emotion:fluff # voice:9
        
        -> p1
    + [...]
        That reminds me... # voice:10
        
        -> p1

= p1
Why'd you come here, anyway? # voice:11

I mean, I'm sure you'd make a lot more driving 'round in a big city. # pre_emotion:fluff # voice:12
    + [I'm visiting a friend.]
        Oh shoot, for real? # pre_emotion:fluff # voice:13
        
        I might know them. # pre_emotion:laugh # voice:14
        
        Y'know... small town and all... # pre_emotion:fluff # voice:15
        
        It can take a while to get to know people, but you'll get used to it. # voice:22
        
        -> p2
    + [I don't know.]
        Going with the flow, I see. # pre_emotion:fluff # voice:16
        
        That's cool. That's cool. # pre_emotion:laugh # voice:17
        
        Welcome to Mackenzie, man. # pre_emotion:fluff # voice:18
        
        It can take a while to get to know people, but you'll get used to it. # voice:22
        
        -> p2
    + [...]
        Not a man of many words, huh? # pre_emotion:fluff # voice:19
        
        I get it, man. I'll let you do your thing. # pre_emotion:laugh # voice:20
        
        Welcome to Mackenzie, man. # pre_emotion:fluff # voice:18
        
        If you need anything, I'll be— # pre_emotion:fluff # voice:21
        
        Ah. I forgot to introduce myself. # pre_emotion:fluff # voice:23
        
        -> p2

= p2

Wait... # voice:24

Have we met before? # pre_emotion:fluff # voice:25

No way. # voice:26

Dominic? # voice:27

It's <b>Daniel</b>—Zay's brother! Do you remember me? # name:Daniel # voice:28
    + [I do.]
        Man, that's crazy! # emotion:laugh # voice:29
        
        Sorry I didn't recognize you earlier! # emotion:default # voice:30
        
        It's good to have you back, man. # pre_emotion:fluff # voice:33
        
        Though I won't lie, I never thought I'd see you again after— # voice:34
        
        Ah. Uh... never mind. Anyways... # pre_emotion:fluff # voice:35
        
        A lot changed after you left. # voice:36
        
        Man... the older I get, the smaller this place feels. # pre_emotion:laugh # voice:37
        
        The stores seem newer, the houses look nicer... Oh— # voice:38
        
        They fixed up the park, too. It's pretty clean now. # spawn_dest # voice:39
        
        You should stop by sometime. # voice:49
        
        Maybe when the sun's out? # pre_emotion:laugh # voice:50
        
        -> p3
    + [I don't.]
        It's chill. # emotion:laugh # voice:31
        
        Been a minute. I don't blame you. # emotion:default # voice:32
        
        It's good to have you back, man. # pre_emotion:laugh # voice:33
        
        Though I won't lie, I never thought I'd see you again after— # voice:34
        
        Ah. Uh... never mind. Anyways... # pre_emotion:fluff # spawn_dest # voice:35
        
        A lot changed after you left. # voice:36
        
        Man... the older I get, the smaller this place feels. # pre_emotion:laugh # voice:37
        
        The stores seem newer, the houses look nicer... Oh— # voice:38
        
        They fixed up the park, too. It's pretty clean now. # voice:39
        
        You should stop by sometime. # voice:49
        
        Maybe when the sun's out? # pre_emotion:laugh # voice:50
        
        -> p3
    + [...]
        Oh. Uh... my bad, man. # emotion:laugh # voice:40
        
        I thought you were someone else. Sorry. # emotion:default # voice:41
        
        I'm sure you get stuff like that all the time. # pre_emotion:laugh # voice:42
        
        It's nice to meet you, though. # voice:43
        
        And man, driving seems fun. # pre_emotion:laugh # voice:44
        
        I'm sure you spend a lot of time going places, huh? # spawn_dest # voice:45
        
        Must be tiring sometimes. # pre_emotion:laugh # voice:46
        
        You ever take some time off to smell the roses? # voice:47
        
        The park's a good place if you're having a slow day. # voice:48
        
        You should stop by sometime. # voice:49
        
        Maybe when the sun's out? # pre_emotion:laugh # voice:50
        
        -> p3

= p3
Speak of the devil. # dropoff # requests_end # voice:51

That's my cue. # voice:52

~ temp mood = GetMood()

{
    - mood > 150:
        Thanks for the ride, man. # voice:53
        
        I hope I catch you again soon! # voice:54
        
    - mood > 50:
        I appreciate the ride, man. # voice:55
        
        Take it easy, alright? # voice:56
    - else:
        See you later, man. # voice:57
        
        Drive safe, yeah? # voice:58
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