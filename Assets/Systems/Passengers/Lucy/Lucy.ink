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

Heya! # name:??? # emotion:default # voice:1

Can you take me to Lucy's Laundry, please? # end_greeting # voice:2

Hm... # requests_start # voice:3

I don't think I've seen you around before. # voice:4

I just moved in a few months ago, but I was certain I'd met almost everyone by now. # voice:5

It's a small town, you know! # voice:6

A taxi driver like you? You should stick out like a sore thumb! Haha! # emotion:laugh # voice:7

You're not new here, are you? # emotion:default # voice:8
    + [I just came back to town.]
        Oh really? # mood:10 # voice:9
        
        I thought no one really left town. # voice:10
        
        <wobble>You didn't just leave, did you?</wobble> # hallucinating:on
        
        <wobble>You ran.</wobble>
        
        <wobble>You couldn't face</wobble> <glitch>her death</glitch>.
        
        Hm? # hallucinating:off # voice:11
        
        ...
        
        You okay? Looks like you're sweating bullets. # voice:12
        
        I- Oh wait! # voice:13
        
        Ah! # voice:14
        
        I do know you! # voice:15
        
        -> p1
    + [No, I'm not.]
        Weird...I've definitely never met you before. # mood:5 # voice:16
        
        ...
        
        You left town recently, didn't you? # voice:17
        
        <wobble>No...you ran.</wobble> # hallucinating:on
        
        <wobble>You couldn't face</wobble> <glitch>her death</glitch>.
        
        Hm? # hallucinating:off # voice:11
        
        ...
        
        You okay? Looks like you're sweating bullets. # voice:12
        
        I- Oh wait! # voice:13
        
        Now that I think about it, I do know you! # voice:22
        
        -> p1
    + [...]
        So, you're the silent type huh? # voice:23
        
        <wobble>You couldn't even say anything when</wobble> <glitch>Maxine died</glitch>. # hallucinating:on
        
        Hm? # hallucinating:off # voice:11
        
        ...
        
        You okay? You're sweating bullets. # voice:25
        
        Did I say something weird? I'm sorry if I did. # voice:26
        
        There's nothing wrong with being quiet y'know! # voice:27
        
        Anyways, I've definitely never met you before. # voice:28
        
        Guess I haven't met everyone yet... # voice:29
        
        Although...this feels kind of... # voice:30
        
        Ah! # voice:14
        
        I do know you! # voice:15
        
        -> p1

= p1
Well, not personally of course. # voice:33

I think it was...Old Man Mc- # voice:34

Mc... # voice:35

Stink? Yea! It was Old Man McStink! # voice:36

He told me about you! # voice:37

You're the guy who left town last year! # voice:38

He never told me why... # voice:39

Oh, but don't worry. I won't pry further! # voice:40

Although...just between you and me, he seems a bit envious of you. # voice:41

He talked about you with a certain look in his eyes. # voice:42

You know the one I'm talking about. # voice:43

When he gets all mopey and wistful. # voice:44

As the two "newcomers" to town, we should get along. # voice:45

It's decided! You and I are going to be best friends now. # emotion:laugh # voice:46

Actually, it's my goal to become friends with everyone in town. # emotion:default # voice:47

It's essential to my job! # emotion:actually # voice:48

Oh wait! I've been yapping all this time and haven't properly introduced myself yet, have I? # emotion:default # voice:49

I'm Lucy! # emotion:actually # name:Lucy # voice:50

... # emotion:default

Eh? No reaction? # voice:51

I-It's me, Lucy! # emotion:actually # voice:52

... # emotion:default

O-owner of the esteemed Lucy's Laundry!? # voice:53

You've heard about it before? R-right?! # voice:54
    + [Not until today.]
        I'll have to face the bitter truth and accept it as fact... # voice:57
        
        -> p2
    + [No.]
        I'll have to face the bitter truth and accept it as fact... # mood:-5 # voice:57
        
        -> p2
    + [...]
        Man... # mood:-5 # voice:55
        
        I'll take that as a no. # voice:56
        
        I'll have to face the bitter truth and accept it as fact... # voice:57
        
        -> p2

= p2
Lucy's Laundry isn't the global, worldwide brand I thought it would be. # voice:58

But mark my words it'll get there! I have an ace hidden up my sleeve. # voice:59

I'll let you in on the secret, now that we're best friends. # voice:60

I've been working on something amazing...pods! Detergent Pods! # emotion:actually # voice:61

They have a water-soluble outer layer with highly concentrated laundry detergent underneath! # emotion:default # voice:62

It's a masterpiece if I do say so myself. And, well, I am myself! # voice:63

It'll revolutionize Big Laundry and take the world by storm! # emotion:laugh # voice:64
    + [...]
        
        -> p3
    + [...]
        
        -> p3
    + [...]
        
        -> p3

= p3
The silent treatment hurts, you know. # emotion:default # voice:65

Seems like it's an idea years ahead of its time... # voice:66

Laundry aside, I'm glad I was able to meet you. # voice:67

I've been trying my best, but it's kind of hard to make friends around here. # voice:68

In fact, I don't think I have any yet... # voice:69

Well, maybe Old Stink McGuy... # voice:70

Anyway, I have a question for you. # voice:71

It's a question that <glitch>only you</glitch> can answer. # voice:72

What do you think about this town? # voice:73
    + [Why?]
        
        -> p4
    + [What do you mean?]
        
        -> p4
    + [...]
        
        -> p4

= p4
Well, I've been doing a bit of thinking recently. # voice:74

Someone... # voice:75

No. # voice:76

Everyone here always says the same exact thing. # voice:77

"This town never changes." "It's the same old Mackenzie it's always been." # voice:78

<glitch>Mackenzie's just that kind of place I guess.</glitch> # voice:79

No one ever leaves. # voice:80

No one ever moves in. # voice:81

Don't get me wrong, I love everyone here, but it's driving me a bit insane. # voice:82

I don't know what they're so afraid of. # voice:83

Everyone's so comfortable being stagnant; it's kind of creepy. # voice:84

It's almost like Mackenzie's frozen in time or something. # voice:85

But if this town never changes, what does that make us? # voice:86

We're kind of anomalies here, don't you think? # voice:87
    + [<s>I-</s> ...]
        
        -> p5
    + [<s>Don't</s> ...]
        
        -> p5
    + [<s>Know</s> ...]
        
        -> p5

= p5
I don't know your situation, but you returned here for a reason. # voice:88

Don't you think we can be the change this town needs? # voice:89

It's just... # voice:90

Even in a place like this, there <b>must</b> be others like us. # voice:91

People who long for something more, people who want to— # voice:92

Ah, sorry. I think I got a bit too excited there. # voice:93

Whoa... # voice:94

I almost sounded cool back there, don't you think? # pre_emotion:laugh # spawn_dest # voice:95

I guess even someone as cute as me can be insightful at times. Hehe. # voice:96

Sorry, I've been talking your ear off this whole time, haven't I? # voice:97

I want to thank you for listening to me. # voice:98

I never knew how much I needed to get this off my chest. # pre_emotion:laugh # voice:99

So... thank you! # voice:100

I can already tell we're going to be besties for life! # voice:101

Well, I'll let you enjoy the peace and quiet of a late-night car ride for now. # pre_emotion:laugh # voice:102

It'll be a nice change of pace—maybe it's something we both need right now. # voice:103

... # dropoff # requests_end

~ temp mood = GetMood()

{
    - mood > 150:
        Here, before I go, I have something for you. Take it! # gift:0 # voice:104
        
        It's exclusive Lucy's Laundry merchandise! # emotion:actually # voice:105
        
        Hehe, it's not even released to the public yet. # emotion:default # voice:106

        I want you to have it. # voice:107
        
        I'll see you around! # voice:108
        
    - mood > 50:
        Thanks for the ride! # voice:109
        
        It was nice having someone to talk with about this kind of stuff. # voice:110
        
        I'll see you around! # voice:108
    - else:
        Thanks for the conversation! # voice:112
        
        I'll catch you some other time! # voice:113
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