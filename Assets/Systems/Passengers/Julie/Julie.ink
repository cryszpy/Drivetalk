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

Thank goodness you're here! Boy, am I glad to see you. # emotion:flirty

Can you take me to 1100 Flower Blvd? The one by the park? # emotion:default # end_greeting

Man, you really saved me. My boyfriend was supposed to pick me up so we could go together, but he said some emergency came up. # requests_start

This isn't the first time this has happened...but he's been so sweet to me so I can't be too mad at him. # emotion:flirty

It hasn't been too long, but things feel new and exciting. # emotion:default

Who knows, maybe he'll stick around for a while. # pre_emotion:laugh

Better than the others by a long shot.

You know, I had to run all the way to your taxi in heels! Really works up a sweat.

I abs-o-lutely can't miss Kim's party! I'm already running behind as is.

I spent a <i>lot</i> of time on my hair and makeup today.

<i>He's</i> not here to appreciate it, but at least you can.

Well, doesn't hurt to show up fashionably late. # pre_emotion:laugh

She'll understand.

...

There's only so much to do in this town. Gotta make the most of my youth or something like that, right?
    + [For sure.]
        Right, right? You get it. # pre_emotion:laugh # mood:20
        
        I can't spend all my days working.
        
        A girl's gotta have fun every now and then!
        
        I'm trying to work a little less. I used to work way more when I was still at the diner...
        
        Now I'm at the bridal boutique on 5th. Every day feels so dreamy! # pre_emotion:laugh
        
        Everyone who walks in looks like a princess. Makes me wonder about my big day, too...whenever that'll be.
        
        -> p1
    + [Something like that.]
        Hmm...I just wanna live my life to the fullest, you know? # mood:10
        
        I feel a little more free since I left the diner.
        
        I used to spend so much time there, all my days were starting to feel like they were blurring together! # emotion:sad
        
        I can't say I enjoyed it, but the money was decent. I got nice tips. # emotion:default
        
        At some point, I couldn't stand it anymore and left!
        
        Now that I'm working at the bridal boutique on 5th, every day feels so dreamy! # pre_emotion:laugh
        
        I've got more to worry about now, but I'm happy and having fun!
        
        That's gotta count for something.
        
        -> p1
    + [...]
        Ahh...anyways... # mood:-10
        
        -> p1

= p1
Been here all my life. Know everyone there is to know.

Haven't seen you around here for a while, but welcome back I guess.

You probably don't remember me, but I was in the year below you.

I would see you hanging out by the gas station after school all the time with that one pretty girlie.

I used to think you two were those cool kids who didn't care about a thing.

Don't worry, I think you're still pretty cool! # pre_emotion:laugh

...

I'm Julie, by the way. Been back long? # name_revealed
    + [You could say that.]
        Ahh, c'mon, give me more than that! Where've you been? # pre_emotion:laugh # mood:20
        
        It's not that often someone gets outta here.
        
        We don't get a lot of taxi drivers out here either, so it's nice that you're here.
        
        Especially when your boyfriend's car breaks down for the umpteenth time this month. Real unfortunate luck. # pre_emotion:laugh
        
        -> p2
    + [More or less.]
        Ahh, c'mon, give me more than that! Where've you been? # pre_emotion:laugh # mood:10
        
        It's not often someone gets outta here.
        
        Well, I do like a man with an air of mystery. # emotion:flirty
        
        Just kidding, don't get the wrong idea now! # emotion:default # pre_emotion:laugh
        
        I think <glitch>someone else</glitch> has your heart right now anyways, right?
        
        ...
        
        -> p2
    + [...]
        ...
        
        I get it, man. I'll let you do your thing. # pre_emotion:laugh
        
        Not much of a talker, huh? That's fine.
        
        -> p2

= p2
Say...do you ever feel like you're running out of time?
    + [Sometimes.]
        Thank goodness you understand! # pre_emotion:laugh # mood:10
        
        Maybe we're more alike than I thought. But...
        
        -> p3
    + [Not really.]
        Well...let me put it like this...
        
        -> p3
    + [...]
        
        -> p3

= p3
I'm sure you've been doing this for a while. Doing fine for yourself and all.

There's so many people my age that are done with school, got good jobs lined up, or even have kids!

I want nothing more than to have a life like that, but...

I keep hopping from job to job.

It's been a long time since I've been in school.

I keep saying I'm planning to go back, but it's becoming harder to convince myself it's ever gonna happen. # emotion:sad

I just don't know what I'm doing wrong. # emotion:default

...

Especially with the news of <glitch>her passing</glitch> last year. You know she was just a little older than me?

Well, of course you knew. What am I saying... # pre_emotion:laugh
    + [I'd rather not speak about this anymore.]
        Sorry, sorry...didn't mean to rant at you. # emotion:sad # mood:-10
        
        Life's just getting to me, is all...
        
        -> p4
    + [Right...]
        Sorry. I didn't mean to dig up old wounds. # emotion:sad
        
        I know how you must have <glitch>felt about her</glitch>...
        
        Life's just getting to me, is all...
        
        -> p4
    + [...]
        Sorry...life's just getting to me, is all... # emotion:sad
        
        -> p4

= p4
I don't mean to get all gloomy on you. # emotion:default

I think I just want to try enjoying life a little more.
    + [You can.]
        That's true. Maybe I'm getting ahead of myself. # spawn_dest
        
        Just gotta take it one day at a time, right?
        
        -> p5
    + [It's not too late.]
        That's true, I guess... # spawn_dest
        
        A part of me feels...stuck. Limited.
        
        -> p5
    + [...]
        Well, I've ranted enough to you now. # spawn_dest
        
        -> p5

= p5
Maybe it's time I get outta here too.

Mackenzie, I mean. Though I guess my stop's coming up too, isn't it...

... # dropoff # requests_end

~ temp mood = GetMood()

{
    - mood > 150:
        Thanks again for the ride...and for listening to me.
        
        I appreciate it, really. More than you know.
        
        If you see me around, don't be a stranger now! # pre_emotion:laugh

        Well, time to turn heads! # emotion:flirty
        
    - mood > 50:
        Thanks again for the ride.
        
        See ya.
    - else:
        Thanks for the ride.
        
        I'll see you around.
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