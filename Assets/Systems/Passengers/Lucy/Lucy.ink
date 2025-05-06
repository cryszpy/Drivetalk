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

Heya! # emotion:default

Can you take me to Lucy's Laundry, please? # end_greeting

Hm... # requests_start

I don't think I've seen you around before.

I just moved in a few months ago, but I was certain I'd met everyone by now.

It's a small town, you know!

A taxi driver like you should stick out like a sore thumb. Haha! # emotion:laugh

You're not new here, are you? # emotion:default
    + [I just came back to town.]
        Oh really? # mood:10
        
        I thought no one really left town.
        
        <wobble>You didn't just leave, did you?</wobble> # hallucinating:on
        
        <wobble>You ran.</wobble>
        
        <wobble>You couldn't face</wobble> <glitch>her death</glitch>.
        
        Hm? # hallucinating:off
        
        ...
        
        You okay? Looks like you're sweating bullets.
        
        I- Oh wait!
        
        Ah!
        
        I do know you!
        
        -> p1
    + [No, I'm not.]
        Weird...I've definitely never met you before. # mood:5
        
        ...
        
        You left town recently, didn't you?
        
        <wobble>No...you ran.</wobble> # hallucinating:on
        
        <wobble>You couldn't face</wobble> <glitch>her death</glitch>.
        
        Hm? # hallucinating:off
        
        ...
        
        You okay? Looks like you're sweating bullets.
        
        I- Oh wait!
        
        Now that I think about it, I do know you!
        
        -> p1
    + [...]
        So, you're the silent type huh?
        
        <wobble>You couldn't even say anything when</wobble> <glitch>Maxine died</glitch>. # hallucinating:on
        
        Hm? # hallucinating:off
        
        ...
        
        You okay? You're sweating bullets.
        
        Did I say something weird? I'm sorry if I did.
        
        Nothing wrong with being quiet y'know!
        
        Anyways, I've definitely never met you before.
        
        Guess I haven't met everyone yet...
        
        Although...this feels kind of...
        
        Ah!
        
        I do know you!
        
        -> p1

= p1
Well, not personally of course.

I think it was...Old Man Mc-

Mc...

Stink? Yea! It was Old Man McStink!

He told me about you!

You're the guy who left town last year!

He never told me why...

Oh, but don't worry. I won't pry further!

Although...just between you and me, he seems a bit envious of you.

He talked about you with a certain look in his eyes.

You know the one I'm talking about.

When he gets all mopey and wistful.

As the two "newcomers" to town, we should get along.

It's decided! You and I are going to be best friends now. # emotion:laugh

Actually, it's my goal to become friends with everyone in town. # emotion:default

It's essential to my job! # emotion:actually

Oh wait! I've been yapping all this time and haven't properly introduced myself yet, have I? # emotion:default

I'm Lucy! # emotion:actually # name_revealed

... # emotion:default

Eh? No reaction?

I-It's me, Lucy! # emotion:actually

... # emotion:default

O-owner of the esteemed Lucy's Laundry!?

You've heard about it before? R-right?!
    + [Not until today.]
        I'll have to face the bitter truth and accept it as fact...
        
        -> p2
    + [No.]
        I'll have to face the bitter truth and accept it as fact... # mood:-5
        
        -> p2
    + [...]
        Man... # mood:-5
        
        I'll take that as a no.
        
        I'll have to face the bitter truth and accept it as fact...
        
        -> p2

= p2
Lucy's Laundry isn't the global, worldwide brand I thought it would be.

But mark my words it'll get there! I have an ace hidden up my sleeve.

I'll let you in on the secret, now that we're best friends.

I've been working on something amazing...pods! Detergent Pods! # emotion:actually

They have a water-soluble outer layer with highly concentrated laundry detergent underneath! # emotion:default

It's a masterpiece if I do say so myself. And, well, I am myself!

It'll revolutionize Big Laundry and take the world by storm! # emotion:laugh
    + [...]
        
        -> p3
    + [...]
        
        -> p3
    + [...]
        
        -> p3

= p3
The silent treatment hurts, you know. # emotion:default

Seems like it's an idea years ahead of its time...

Laundry aside, I'm glad I was able to meet you.

I've been trying my best, but it's kind of hard to make friends around here.

In fact, I don't think I have any yet...

Well, maybe Old Stink McGuy...

Anyway, I have a question for you.

It's a question that <glitch>only you</glitch> can answer.

What do you think about this town?
    + [Why?]
        
        -> p4
    + [What do you mean?]
        
        -> p4
    + [...]
        
        -> p4

= p4
Well, I've been doing a bit of thinking recently.

Someone...

No.

Everyone here always says the same thing.

"This town never changes." "It's the same old Mackenzie it's always been."

<glitch>Mackenzie's just that kind of place I guess.</glitch>

No one ever leaves.

No one ever moves in.

Don't get me wrong, I love everyone here, but it's driving me a bit insane.

I don't know what they're so afraid of.

Everyone's so comfortable being stagnant; it's kind of creepy.

It's almost like Mackenzie's frozen in time or something.

But if this town never changes, what does that make us?

We're kind of anomalies here, don't you think?
    + [<s>I-</s> ...]
        
        -> p5
    + [<s>Don't</s> ...]
        
        -> p5
    + [<s>Know</s> ...]
        
        -> p5

= p5
I don't know your situation, but you returned here for a reason.

Maybe we can be the change this town needs!

Also I-

Ah, sorry. I think I'm getting a bit sidetracked.

Whoa, I almost sounded cool back there. # spawn_dest

I guess even someone as cute as me can be insightful at times. Hehe. # pre_emotion:laugh

I'll let you enjoy the quiet of a late-night car ride for now. # pre_emotion:laugh

It'll be a nice change of pace—maybe it's something we both need right now.

... # dropoff # requests_end

~ temp mood = GetMood()

{
    - mood > 150:
        Here, before I go, I have something for you. Take it! # gift:0
        
        It's exclusive Lucy's Laundry merchandise! # emotion:actually
        
        Hehe, it's not even released to the public yet. # emotion:default

        I want you to have it.
        
        I'll see you around!
        
    - mood > 50:
        Thanks for the ride!
        
        It was nice having someone to talk with about this kind of stuff.
        
        I'll see you around!
    - else:
        Thanks for the conversation!
        
        I'll catch you some other time!
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