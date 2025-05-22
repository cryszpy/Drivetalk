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

Hey! # emotion:default # name:Maxine # voice:1

Do you mind dropping me off at 270 Ardendale? # voice:2

...

Er- Do you know where that is? # voice:3

...

It's okay! Feel free to use the <b>GPS</b> on your dashboard! # end_greeting # voice:4

You won't make it as a taxi driver if you don't know where you're going. # emotion:laughflowy # voice:5

It's a harsh world out there for you taxi drivers! # emotion:default # voice:6

Lots of competition, y'know? # voice:7

Well, maybe. # voice:8

I'm actually not too sure myself. # emotion:default # pre_emotion:laughflowy # voice:9

Consider it a pet theory of mine. # voice:10

It seems true though, don't you think? # voice:11
    + [Well...I guess so.]
        Hmm... # voice:12
        
        I couldn't tell it was you from back here, but that voice... # voice:13
        
        -> p1
    + [Around this town? Probably not.]
        Hmm... # voice:12
        
        I couldn't tell it was you from back here, but that voice... # voice:13
        
        -> p1
    + [...]
        Hmm... # voice:12
        
        I couldn't tell it was you from back here, but that silence... # emotion:cesmile # voice:96
        
        -> p1

= p1
<b>Dom?!</b> # emotion:default # voice:14

I heard you were back in town, but I didn't believe it. # voice:15

You've almost been gone for a full year y'know? # voice:16

...

So you're a taxi driver now huh? # pre_emotion:cesmile # voice:17

Honestly, this job seems to suit you well. # voice:18

Actually...now that I'm looking closer— # voice:19

This is your old car from way back then, isn't it? # voice:20

Y'know, I remember how broken it used to be. # voice:21

Your <b>AC</b> was all rickety and <b>couldn't keep a temperature...</b> # requests_start # voice:22

Your <b>radio</b> would <b>cut in and out...</b> # voice:23

Your <b>hazard lights</b> used to turn <b>on randomly...</b> # voice:24

All of <b>your passengers are gonna have a preference</b> for that kind of thing y'know? # voice:25

Make sure everything is set to <b>maximize their comfort!</b> # voice:26

<b>Plus, if things start breaking, you gotta fix it!</b> # voice:27

Honestly, it's a surprise this piece of junk still drives. # emotion:laughflowy # voice:28

Don't get me wrong, the paint job really did wonders. # emotion:default # voice:29

It definitely feels like a taxi. # voice:30

But, maybe you should've paid for some maintenance first haha! # emotion:laughflowy # voice:31

Driving around like this...It's a bit nostalgic isn't it? Well, at least it is to me. # emotion:default # pre_emotion:cesmile # voice:32

Do you remember when you first got your license? # voice:33
    + [Of course I do.]
        Haha! It was like I was seeing a whole different side of you! # pre_emotion:laughflowy # mood:10 # voice:34
        
        I'd never seen you giddy like that before. # voice:35
        
        -> p2
    + [I don't remember...]
        Don’t get shy now you liar, I know you remember. # emotion:pouty # voice:36
        
        -> p2
    + [...]
        Your silence speaks volumes Dom! # voice:37
        
        I know you remember. # emotion:laughflowy # voice:38
        
        -> p2

= p2
You'd come to my house every day and drive me around town for hours. # emotion:default # voice:39

I wouldn't know when you'd come. From dusk to dawn—it was really whenever. # voice:40

I just remember talking about our lives, our dreams, and the future. # voice:41

Honestly, you're lucky my mom's known you your whole life. # voice:42

Otherwise, she would've killed you. # emotion:laughflowy # voice:43

She told me you shouldn't play with a young maiden's heart like that! # emotion:default # voice:44

I agree with her, y'know! # emotion:pouty # voice:45

... # emotion:default

As much as I like to tease, it really is nice to see you again! # pre_emotion:cesmile # pause:1.75 # voice:46

Y'know, the town hasn't changed much since you've been gone. # voice:47

<b>Mackenzie's</b> just that kind of place I guess. # voice:48

...

I can kind of understand why you left. # voice:49

<glitch>Especially, after what happened to—</glitch> # voice:50

Well, y'know. # voice:51

The same ol' hustle and bustle leaves a bitter taste in your mouth. # voice:52

The familiar slowly becomes unfamiliar... # voice:53

It's a strange feeling. # voice:54

...

Sorry, maybe we should talk about this next time. # pre_emotion:cesmile # spawn_dest # voice:55

Plus, my stop is coming up soon. # voice:56

Let's do this again sometime. I had a lot of fun! # voice:57

I'm glad that I got to see you again after so long! # voice:58

Hopefully, you feel the same way! Hehe. # voice:59

Well, this is me. # dropoff # requests_end # voice:60

<glitch>It's been a while since you've been here hasn't it?</glitch> # voice:61

I'll see you around! # voice:62

'Till next time, Dom! # voice:63

~ PostDropoff() // Drops passenger off at destination
-> DONE

=== _2 ===
~ UpdateCurrentKnot("_2")

<glitch>Hey!</glitch> # emotion:default # name:Maxine # hallucinating:on # voice:97

<glitch>Do you mind dropping me off at 270 Ardendale?</glitch> # voice:98

<glitch>...</glitch>

<glitch>Er- Do you know where that is?</glitch> # voice:99

<glitch>...</glitch>

<glitch>It's okay! Feel free to use the GPS on your dashboard!</glitch> # end_greeting # voice:100

You won't make it <glitch>as a taxi driver if</glitch> you don't know where you're going. # emotion:laughflowy # voice:101

It's a harsh world <glitch>out there</glitch> for you <glitch>taxi drivers!</glitch> # voice:102

<glitch>Lots of competition, y'know?</glitch> # voice:103

<glitch>Well, maybe.</glitch> # voice:104

I'm <glitch>actually</glitch> not <glitch>too sure</glitch> myself. # pre_emotion:laughflowy # voice:105

<glitch>Consider it a pet theory of mine.</glitch> # voice:106

<glitch>It seems true though, don't you think?</glitch> # voice:107
    + [<s>She's</s>]
        <glitch>Hmm...</glitch> # voice:108
        
        <glitch>I couldn't tell</glitch> it was you <glitch>from back here, but that voice...</glitch> # voice:109
        
        -> p1
    + [<s>not</s>]
        <glitch>Hmm...</glitch> # voice:108
        
        <glitch>I couldn't tell</glitch> it was you <glitch>from back here, but that voice...</glitch> # voice:109
        
        -> p1
    + [<s>herself?</s>]
        <glitch>Hmm...</glitch> # voice:108
        
        <glitch>I couldn't tell</glitch> it was you <glitch>from back here, but that silence...</glitch> # emotion:cesmile # voice:134
        
        -> p1

= p1
<b>Dom?!</b> # emotion:default # voice:14

<glitch>I heard</glitch> you <glitch>were back in town, but I</glitch> didn't believe <glitch>it.</glitch> # voice:111

You've <glitch>almost</glitch> been gone <glitch>for a full year y'know?</glitch> # voice:112
    + [<s>I've</s>]

        -> p2
    + [<s>been</s>]

        -> p2
    + [<s>gone...</s>]

        -> p2
        
= p2
<glitch>...</glitch>

<glitch>So you're a taxi driver now huh?</glitch> # voice:113

<glitch>Honestly, this job seems to suit you well.</glitch> # voice:114

<glitch>Actually, now that I'm</glitch> look<glitch>ing</glitch> closer<glitch>...</glitch> # voice:115
    + [<s>What am I</s>]

        -> p3
    + [<s>looking</s>]

        -> p3
    + [<s>at...</s>]

        -> p3

= p3
<glitch>This is your old car from way back then, isn't it?</glitch> # voice:116

<glitch>Driving around like this...</glitch>It's<glitch>a bit nostalgic isn't it? Well, at least it is to</glitch> me. # voice:117
    + [<s>I</s>]

        -> p4
    + [<s>miss</s>]

        -> p4
    + [<s>you...</s>]

        -> p4

= p4
Do you remember <glitch>when you first got your license</glitch>? # voice:118
    + [<s>I'll</s>]
        <glitch>Haha! It was like I was seeing a whole different side of you!</glitch> # emotion:laughflowy # voice:119
        
        I'd never seen you <glitch>giddy</glitch> like that before. # emotion:default # voice:120

        -> p5
    + [<s>always</s>]
        <glitch>Don't get shy now you liar,</glitch> I know you remember. # emotion:pouty # voice:121

        -> p5
    + [<s>remember</s>]
        <glitch>Your silence speaks volumes, Dom!</glitch> # voice:122
        
        I know you remember. # pre_emotion:laughflowy # voice:38

        -> p5

= p5
<glitch>You'd come to my house every day and drive me around town for hours.</glitch> # emotion:default # voice:124

I wouldn't <glitch>know when you'd come. From dusk to dawn—it was really whenever.</glitch> # voice:125

<glitch>I just remember</glitch> talk<glitch>ing about our lives, our dreams, and the future.</glitch> # voice:126

<glitch>Honestly, you're lucky my mom's known you your whole life.</glitch> # voice:127

<glitch>Otherwise, she would've killed you.</glitch> # emotion:laughflowy # voice:128

<glitch>She told me you shouldn't play with a young maiden's heart</glitch> like that<glitch>!</glitch> # emotion:default # voice:129

<glitch>I agree with her, y'know!</glitch> # emotion:pouty # voice:130

<glitch>...</glitch> # emotion:default

<glitch>As much as I like</glitch> to <glitch>tease, it really is nice to see</glitch> you <glitch>again!</glitch> # voice:131
    + [<s>Ah,</s>]

        -> p6
    + [<s>I'm</s>]

        -> p6
    + [<s>sorry</s>]

        -> p6

= p6
Y'know, the town hasn't changed much since you've been gone. # hallucinating:off # voice:47

<b>Mackenzie's</b> just that kind of place I guess. # voice:48
    + [I guess it is.]
        Hmm...It's kind of weird. # voice:64
        
        For some reason, I expected a diferent answer. # pre_emotion:laughflowy # voice:65
        
        Well, this is fine too. # voice:66
        
        Change doesn't come easy y'know? # voice:67
        
        Especially in a town like this. # voice:68
        
        Not everything needs to change. # voice:69
        
        Sometimes it's nice to find peace in the familiar. # voice:70
        
        No matter what path you pick, I'll always be there to support you. # voice:71
        
        Although...I don't think your journey is quite over yet. # voice:72
        
        I— # voice:73
        
        ...
        
        Looks like it's time for me to go. # voice:74
        
        I'll see you soon Dom! # requests_end # kick_out # voice:75

        -> DONE
    + [No, it's not.]
        Oh? # voice:76
            ** [The town seems to be changing.]
                
                -> p7
            
            ** [I've met some interesting people today.]
                
                -> p7
            
            ** [There was this weird girl...She just moved in recently.]
                
                -> p7

= p7
Well, that's interesting. It kind of makes me happy to hear. # pre_emotion:laughflowy # voice:79

Before I go, I just wanted to thank you for everything. # voice:80

I don't know the next time I'll see you, but it'll probably be a while. # voice:81

Don't forget about me okay? # voice:82

...

...

I— # voice:73

I wish I could have met Lucy. # voice:84

I wish I could have talked more with Old Man McGee. # voice:85

I wish I could have gone to Kim's party. # voice:86

I wish I could have pet Romeo another time. # voice:87

I wish— # voice:88

I want— # voice:89

...
    + [Lucy would've liked you.]
        

        -> p8
    + [Romeo would've loved that.]
        
        -> p8
        
    + [Even now, McGee still talks to you.]
        
        -> p8

= p8
Thank you. You always know the right thing to say even though you're silent half the time... # emotion:pouty # voice:90

Although, that's just what makes you, you. # emotion:default # pre_emotion:laughflowy # voice:91

Well... # voice:92

Looks like it's time for me to go. # pre_emotion:cesmile # voice:93

Goodbye, Dom! # voice:94

<glitch>I love you!</glitch> # kick_out # requests_end # voice:95

-> DONE

=== _3 ===
~ UpdateCurrentKnot("_3")

heyyyy what's up tyhree! # larie:default

this shouldn't change!

second line in three! # fallow:default # name_revealed
-> DONE









-> END