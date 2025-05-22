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

Thank goodness you're here! Boy, am I glad to see you. # name:??? # emotion:flirty # voice:1

Can you take me to 1100 Flower Blvd? The one by the park? # emotion:default # end_greeting # voice:2

Man, you really saved me.  # voice:3

My boyfriend was supposed to pick me up so we could go together, but he said some emergency came up. # requests_start # voice:4

This isn't the first time this has happened... # voice:5

...but he's been so sweet to me so I can't be too mad at him. # emotion:flirty # voice:6

It hasn't been too long, but things feel new and exciting. # emotion:default # voice:7

Who knows, maybe he'll stick around a while. # pre_emotion:laugh # voice:8

Better than the others by a long shot. # voice:9

You know, I had to run all the way to your taxi in heels! Really works up a sweat. # voice:10

I abs-o-lutely can't miss Kim's party! I'm already running behind as is. # voice:11

I spent a <i>lot</i> of time on my hair and makeup today. # voice:12

<i>He's</i> not here to appreciate it, but at least you can. # voice:13

Well, doesn't hurt to show up fashionably late. # pre_emotion:laugh # voice:14

She'll understand. # voice:15

...

There's only so much to do in this town. Gotta make the most of my youth or something like that, right? # voice:16
    + [For sure.]
        Right, right? You get it. # pre_emotion:laugh # mood:20 # voice:17
        
        I can't spend all my days working. # voice:18
        
        A girl's gotta have fun every now and then! # voice:19
        
        I'm trying to work a little less. I used to work way more when I was still at the diner... # voice:20
        
        Now I'm at the bridal boutique on 5th. Every day feels so dreamy! # pre_emotion:laugh # voice:21
        
        Everyone who walks in looks like a princess. # voice:22
        
        Makes me wonder about my big day, too...whenever that'll be. # voice:23
        
        -> p1
    + [Something like that.]
        Hmm...I just wanna live my life to the fullest, you know? # mood:10 # voice:24
        
        I feel a little more free since I left the diner. # voice:25
        
        I used to spend so much time there, all my days were starting to feel like they were blurring together! # emotion:sad # voice:26
        
        I can't say I enjoyed it, but the money was decent. I got nice tips. # emotion:default # voice:27
        
        At some point, I couldn't stand it anymore and left! # voice:28
        
        Now that I'm working at the bridal boutique on 5th, every day feels so dreamy! # pre_emotion:laugh # voice:21
        
        I've got more to worry about now, but I'm happy and having fun! # voice:30
        
        That's gotta count for something. # voice:31
        
        -> p1
    + [...]
        Ahh...anyways... # mood:-10 # voice:32
        
        -> p1

= p1
Been here all my life. Know everyone there is to know. # voice:33

Haven't seen you around here for a while, but welcome back I guess. # voice:34

You probably don't remember me, but I was in the year below you. # voice:35

I would see you hanging out by the gas station after school all the time with that one pretty girlie. # voice:36

I used to think you two were those cool kids who didn't care about a thing. # voice:37

Don't worry, I think you're still pretty cool! # pre_emotion:laugh # voice:38

...

I'm Julie, by the way. Been back long? # name:Julie # voice:39
    + [You could say that.]
        Ahh, c'mon, give me more than that! Where've you been? # pre_emotion:laugh # mood:20 # voice:40
        
        It's not that often someone gets outta here. # voice:41
        
        We don't get a lot of taxi drivers out here either, so it's nice that you're here. # voice:42
        
        Especially when your boyfriend's car breaks down for the umpteenth time this month. # voice:43
        
        Real unfortunate luck. # pre_emotion:laugh # voice:44
        
        -> p2
    + [More or less.]
        Ahh, c'mon, give me more than that! Where've you been? # pre_emotion:laugh # mood:10 # voice:40
        
        It's not often someone gets outta here. # voice:41
        
        Well, I do like a man with an air of mystery. # emotion:flirty # voice:47
        
        Just kidding, don't get the wrong idea now! # emotion:default # pre_emotion:laugh # voice:48
        
        I think <glitch>someone else</glitch> has your heart right now anyways, right? # voice:49
        
        ...
        
        -> p2
    + [...]
        ...
        
        Not much of a talker, huh? That's fine. # voice:50
        
        -> p2

= p2
Say...do you ever feel like you're running out of time? # voice:51
    + [Sometimes.]
        Thank goodness you understand! # pre_emotion:laugh # mood:10 # voice:52
        
        Maybe we're more alike than I thought. But... # voice:53
        
        -> p3
    + [Not really.]
        Well...let me put it like this... # voice:54
        
        -> p3
    + [...]
        
        -> p3

= p3
I'm sure you've been doing this for a while. Doing fine for yourself and all. # voice:55

There's so many people my age that are done with school... # voice:56

...got good jobs lined up, or even have kids! # voice:57

I want nothing more than to have a life like that, but... # voice:58

I keep hopping from job to job. # voice:59

It's been a long time since I've been in school. # voice:60

I keep saying I'm planning to go back... # voice:61

...but it's becoming harder to convince myself it's ever gonna happen. # emotion:sad # voice:62

I just don't know what I'm doing wrong. # emotion:default # voice:63

...

Especially with the news of <glitch>her passing</glitch> last year. # voice:64

You know she was just a little older than me? # voice:65

Well, of course you knew. What am I saying... # pre_emotion:laugh # voice:66
    + [I'd rather not speak about this anymore.]
        Sorry, sorry...didn't mean to rant at you. # emotion:sad # mood:-10 # voice:67
        
        Life's just getting to me, is all... # voice:68
        
        -> p4
    + [Right...]
        Sorry. I didn't mean to dig up old wounds. # emotion:sad # voice:69
        
        I know how you must have <glitch>felt about her</glitch>... # voice:70
        
        Life's just getting to me, is all... # voice:68
        
        -> p4
    + [...]
        Sorry...life's just getting to me, is all... # emotion:sad # voice:68
        
        -> p4

= p4
I don't mean to get all gloomy on you. # emotion:default # voice:72

I think I just want to try enjoying life a little more. # voice:73
    + [You can.]
        That's true. Maybe I'm getting ahead of myself. # spawn_dest # voice:74
        
        Just gotta take it one day at a time, right? # voice:75
        
        -> p5
    + [It's not too late.]
        That's true, I guess... # spawn_dest # voice:76
        
        A part of me feels...stuck. Limited. # voice:77
        
        -> p5
    + [...]
        Well, I've ranted enough to you now. # spawn_dest # voice:78
        
        -> p5

= p5
Maybe it's time I get outta here too. # voice:79

Mackenzie, I mean. Though I guess my stop's coming up too, isn't it... # voice:80

... # dropoff # requests_end

~ temp mood = GetMood()

{
    - mood > 150:
        Thanks again for the ride...and for listening to me. # voice:81
        
        I appreciate it, really. More than you know. # voice:82
        
        If you see me around, don't be a stranger now! # emotion:flirty # voice:83
        
    - mood > 50:
        Thanks again for the ride. # voice:84
        
        See ya. # voice:85
    - else:
        Thanks for the ride. # voice:86
        
        I'll see you around. # voice:87
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