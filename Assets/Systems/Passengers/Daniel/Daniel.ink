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

Yo! What's up Taxi-man? # emotion:default

You got time for a ride to the park?

I appreciate it! # end_greeting

Oh. Y'know... # pre_emotion:fluff # pause:5 # requests_start

When I heard a taxi driver was coming to town, I was stoked.

'Cause dude. Don't get me wrong—it's a small town, but walking takes

F O R E V E R.

So I'm glad you're here, man.
    + [Thanks.]
        Of course, man. But I'm curious... # pre_emotion:fluff
        
        -> p1
    + [...]
        That reminds me...
        
        -> p1

= p1
Why'd you come here, anyway?

I mean, I'm sure you'd make a lot more driving 'round in a big city. # pre_emotion:fluff
    + [I'm visiting a friend.]
        Oh shoot, for real? # pre_emotion:fluff
        
        I might know them. # pre_emotion:laugh
        
        Y'know... small town and all... # pre_emotion:fluff
        
        It can take a while to get to know people, but you'll get used to it.
        
        -> p2
    + [I don't know.]
        Going with the flow, I see. # pre_emotion:fluff
        
        That's cool. That's cool. # pre_emotion:laugh
        
        Welcome to Mackenzie, man. # pre_emotion:fluff
        
        It can take a while to get to know people, but you'll get used to it.
        
        -> p2
    + [...]
        Not a man of many words, huh? # pre_emotion:fluff
        
        I get it, man. I'll let you do your thing. # pre_emotion:laugh
        
        Welcome to Mackenzie, man. # pre_emotion:fluff
        
        If you need anything, I'll be— # pre_emotion:fluff
        
        Ah. I forgot to introduce myself. # pre_emotion:fluff
        
        -> p2

= p2

Wait...

Have we met before? # pre_emotion:fluff

No way.

Dominic?

It's <b>Daniel</b>—Zay's brother! Do you remember me? # name_revealed
    + [I do.]
        Man, that's crazy! # emotion:laugh
        
        Sorry I didn't recognize you earlier! # emotion:default
        
        It's good to have you back, man. # pre_emotion:fluff
        
        Though I won't lie, I never thought I'd see you again after—
        
        Ah. Uh... never mind. Anyways... # pre_emotion:fluff
        
        A lot changed after you left.
        
        Man... the older I get, the smaller this place feels. # pre_emotion:laugh
        
        The stores seem newer, the houses look nicer... Oh—
        
        They fixed up the park, too. It's pretty clean now. # spawn_dest
        
        You should stop by sometime.
        
        Maybe when the sun's out? # pre_emotion:laugh
        
        -> p3
    + [I don't.]
        It's chill. # emotion:laugh
        
        Been a minute. I don't blame you. # emotion:default
        
        It's good to have you back, man. # pre_emotion:laugh
        
        Though I won't lie, I never thought I'd see you again after—
        
        Ah. Uh... never mind. Anyways... # pre_emotion:fluff # spawn_dest
        
        A lot changed after you left.
        
        Man... the older I get, the smaller this place feels. # pre_emotion:laugh
        
        The stores seem newer, the houses look nicer... Oh—
        
        They fixed up the park, too. It's pretty clean now.
        
        You should stop by sometime.
        
        Maybe when the sun's out? # pre_emotion:laugh
        
        -> p3
    + [...]
        Oh. Uh... my bad, man. # emotion:laugh
        
        I thought you were someone else. Sorry. # emotion:default
        
        I'm sure you get stuff like that all the time. # pre_emotion:laugh
        
        It's nice to meet you, though.
        
        An man, driving seems fun. # pre_emotion:laugh
        
        I'm sure you spend a lot of time going places, huh? # spawn_dest
        
        Must be tiring sometimes. # pre_emotion:laugh
        
        You ever take some time off to smell the roses?
        
        The park's a good place if you're having a slow day.
        
        -> p3

= p3
Speak of the devil. # dropoff # requests_end

That's my cue.

~ temp mood = GetMood()

{
    - mood > 150:
        Thanks for the ride, man.
        
        I hope I catch you again soon!
        
    - mood > 50:
        I appreciate the ride, man.
        
        Take it easy, alright?
    - else:
        See you later, man.
        
        Drive safe, yeah?
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