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

Hey! # emotion:default # name_revealed

Do you mind <link="hey brother">dropping</link> me off at 270 Ardendale?

...

Er- Do you know where that is?

...

It's okay! Feel free to use the <b>GPS</b> on your dashboard! # end_greeting

You won't make it as a taxi driver if you don't know where you're going. # emotion:laughflowy

It's a harsh world out there for you taxi drivers! # emotion:default

Lots of competition, y'know?

Well, maybe.

I'm actually not too sure myself. # emotion:default # pre_emotion:laughflowy

Consider it a pet theory of mine.

It seems true though, don't you think?
    + [Well...I guess so.]
        Hmm...
        
        I couldn't tell it was you from back here, but that voice...
        
        -> p1
    + [Around this town? Probably not.]
        Hmm...
        
        I couldn't tell it was you from back here, but that voice...
        
        -> p1
    + [...]
        Hmm...
        
        I couldn't tell it was you from back here, but that silence... # emotion:cesmile
        
        -> p1

= p1
<b>Dom?!</b> # emotion:default

I heard you were back in town, but I didn't believe it.

You've almost been gone for a full year y'know?

...

So you're a taxi driver now huh? # pre_emotion:cesmile

Honestly, this job seems to suit you well.

Actually...now that I'm looking closer—

This is your old car from way back then, isn't it?

Y'know, I remember how broken it used to be.

Your <b>AC</b> was all rickety and <b>couldn't keep a temperature...</b> # requests_start

Your <b>radio</b> would <b>cut in and out...</b>

Your <b>hazard lights</b> used to turn <b>on randomly...</b>

All of <b>your passengers are gonna have a preference</b> for that kind of thing y'know?

Make sure everything is set to <b>maximize their comfort!</b>

<b>Plus, if things start breaking, you gotta fix it!</b>

Honestly, it's a surprise this piece of junk still drives. # emotion:laughflowy

Don't get me wrong, the paint job really did wonders. # emotion:default

It definitely feels like a taxi.

But, maybe you should've paid for some maintenance first haha! # emotion:laughflowy

Driving around like this...It's a bit nostalgic isn't it? # emotion:default

Well, at least it is to me. # pre_emotion:cesmile

Do you remember when you first got your license?
    + [Of course I do.]
        Haha! It was like I was seeing a whole different side of you! # pre_emotion:laughflowy # mood:10
        
        I'd never seen you giddy like that before.
        
        -> p2
    + [I don't remember...]
        Don’t get shy now you liar, I know you remember. # emotion:pouty
        
        -> p2
    + [...]
        Your silence speaks volumes Dom!
        
        I know you remember. # emotion:laughflowy
        
        -> p2

= p2
You'd come to my house every day and drive me around town for hours. # emotion:default

I wouldn't know when you'd come. From dusk to dawn—it was really whenever.

I just remember talking about our lives, our dreams, and the future.

Honestly, you're lucky my mom's known you your whole life.

Otherwise, she would've killed you. # emotion:laughflowy

She told me you shouldn't play with a young maiden's heart like that! # emotion:default

I agree with her, y'know! # emotion:pouty

... # emotion:default

As much as I like to tease, it really is nice to see you again! # pre_emotion:cesmile # pause:1.75

Y'know, the town hasn't changed much since you've been gone.

<b>Mackenzie's</b> just that kind of place I guess.

...

I can kind of understand why you left.

<glitch>Especially, after what happened to—</glitch>

Well, y'know.

The same ol' hustle and bustle leaves a bitter taste in your mouth.

The familiar slowly becomes unfamiliar...

It's a strange feeling.

...

Sorry, maybe we should talk about this next time. # pre_emotion:cesmile # spawn_dest

Plus, my stop is coming up soon.

Let's do this again sometime. I had a lot of fun!

I'm glad that I got to see you again after so long!

Hopefully, you feel the same way! Hehe.

Well, this is me. # dropoff # requests_end

<glitch>It's been a while since you've been here hasn't it?</glitch>

I'll see you around!

'Till next time, Dom!

~ PostDropoff() // Drops passenger off at destination
-> DONE

=== _2 ===
~ UpdateCurrentKnot("_2")

<glitch>Hey!</glitch> # emotion:default # name_revealed # hallucinating:on

<glitch>Do you mind dropping me off at 270 Ardendale?</glitch>

<glitch>...</glitch>

<glitch>Er- Do you know where that is?</glitch>

<glitch>...</glitch>

<glitch>It's okay! Feel free to use the GPS on your dashboard!</glitch> # end_greeting

You won't make it <glitch>as a taxi driver if</glitch> you don't know where you're going. # emotion:laughflowy

It's a harsh world <glitch>out there</glitch> for you <glitch>taxi drivers!</glitch>

<glitch>Lots of competition, y'know?</glitch>

<glitch>Well, maybe.</glitch>

I'm <glitch>actually</glitch> not <glitch>too sure</glitch> myself. # pre_emotion:laughflowy

<glitch>Consider it a pet theory of mine.</glitch>

<glitch>It seems true though, don't you think?</glitch>
    + [<s>She's</s>]
        <glitch>Hmm...</glitch>
        
        <glitch>I couldn't tell</glitch> it was you <glitch>from back here, but that voice...</glitch>
        
        -> p1
    + [<s>not</s>]
        <glitch>Hmm...</glitch>
        
        <glitch>I couldn't tell</glitch> it was you <glitch>from back here, but that voice...</glitch>
        
        -> p1
    + [<s>herself?</s>]
        <glitch>Hmm...</glitch>
        
        <glitch>I couldn't tell</glitch> it was you <glitch>from back here, but that silence...</glitch> # emotion:cesmile
        
        -> p1

= p1
<glitch>...</glitch> # emotion:default

<glitch>So you're a taxi driver now huh?</glitch>

<glitch>Honestly, this job seems to suit you well.</glitch>

<glitch>Actually, now that I'm</glitch> look<glitch>ing</glitch> closer<glitch>...</glitch>
    + [<s>What am I</s>]

        -> p2
    + [<s>looking</s>]

        -> p2
    + [<s>at...</s>]

        -> p2

= p2
<glitch>This is your old car from way back then, isn't it?</glitch>

<glitch>Driving around like this...</glitch>It's<glitch>a bit nostalgic isn't it? Well, at least it is to</glitch> me.
    + [<s>I</s>]

        -> p3
    + [<s>miss</s>]

        -> p3
    + [<s>you...</s>]

        -> p3

= p3
Do you remember <glitch>when you first got your license</glitch>?
    + [<s>I'll</s>]
        <glitch>Haha! It was like I was seeing a whole different side of you!</glitch> # emotion:laughflowy
        
        I'd never seen you <glitch>giddy</glitch> like that before. # emotion:default

        -> p4
    + [<s>always</s>]
        <glitch>Don't get shy now you liar,</glitch> I know you remember. # emotion:pouty

        -> p4
    + [<s>remember</s>]
        <glitch>Your silence speaks volumes, Dom!</glitch>
        
        I know you remember. # pre_emotion:laughflowy

        -> p4

= p4
<glitch>You'd come to my house every day and drive me around town for hours.</glitch> # emotion:default

I wouldn't <glitch>know when you'd come. From dusk to dawn—it was really whenever.</glitch>

<glitch>I just remember</glitch> talk<glitch>ing about our lives, our dreams, and the future.</glitch>

<glitch>Honestly, you're lucky my mom's known you your whole life.</glitch>

<glitch>Otherwise, she would've killed you.</glitch> # emotion:laughflowy

<glitch>She told me you shouldn't play with a young maiden's heart</glitch> like that<glitch>!</glitch> # emotion:default

<glitch>I agree with her, y'know!</glitch> # emotion:pouty

<glitch>...</glitch> # emotion:default

<glitch>As much as I like</glitch> to <glitch>tease, it really is nice to see</glitch> you <glitch>again!</glitch>
    + [<s>Ah,</s>]

        -> p5
    + [<s>I'm</s>]

        -> p5
    + [<s>sorry</s>]

        -> p5

= p5
Y'know, the town hasn't changed much since you've been gone. # hallucinating:off

<b>Mackenzie's</b> just that kind of place I guess.
    + [I guess it is.]
        Hmm...It's kind of weird.
        
        For some reason, I expected a diferent answer. # pre_emotion:laughflowy
        
        Well, this is fine too.
        
        Change doesn't come easy y'know?
        
        Especially in a town like this.
        
        Not everything needs to change.
        
        Sometimes it's nice to find peace in the familiar.
        
        No matter what path you pick, I'll always be there to support you.
        
        Although...I don't think your journey is quite over yet.
        
        I—
        
        ...
        
        Looks like it's time for me to go.
        
        I'll see you soon Dom! # requests_end # kick_out

        -> DONE
    + [No, it's not.]
        Oh?
            ** [The town seems to be changing.]
                
                -> p6
            
            ** [I've met some interesting people today.]
                
                -> p6
            
            ** [There was this weird girl...She just moved in recently.]
                
                -> p6

= p6
Well, that's interesting. It kind of makes me happy to hear. # pre_emotion:laughflowy

Before I go, I just wanted to thank you for everything.

I don't know the next time I'll see you, but it'll probably be a while.

Don't forget about me okay?

...

...

I—

I wish I could have met Lucy.

I wish I could have talked more with Old Man McGee.

I wish I could have gone to Kim's party.

I wish I could have pet Romeo another time.

I wish—

I want—

...
    + [Lucy would've liked you.]
        

        -> p7
    + [Romeo would've loved that.]
        
        -> p7
        
    + [Even now, McGee still talks to you.]
        
        -> p7

= p7
Thank you. You always know the right thing to say even though you're silent half the time... # emotion:pouty

Although, that's just what makes you, you. # emotion:default # pre_emotion:laughflowy

Well...

Looks like it's time for me to go. # pre_emotion:cesmile

Goodbye, Dom!

<glitch>I love you!</glitch> # kick_out # requests_end

-> DONE

=== _3 ===
~ UpdateCurrentKnot("_3")

heyyyy what's up tyhree! # larie:default

this shouldn't change!

second line in three! # fallow:default # name_revealed
-> DONE









-> END