# Unity Setup Guide: PlayerCube Quest-Specific Animations

## 🎯 Overview
This guide will help you set up the Animator Controller for the PlayerCube to support different animations based on quest types and player states.

## 📋 Required Setup Steps

### 1. **Create Animator Controller**
1. In Project window, right-click → Create → Animator Controller
2. Name it `PlayerCubeAnimator`
3. Place it in `Assets/Animations/` folder

### 2. **Setup PlayerCube GameObject**
1. Find the `PlayerCube` GameObject in your scene
2. Add `Animator` component if not present
3. Assign the `PlayerCubeAnimator` controller to the Animator
4. Ensure the `Player.cs` script can find this object (name should be "PlayerCube")

### 3. **Create Animation States**

#### **Base States:**
- **Idle** - Default rotation when no quest is active
- **Gather** - Slower, rhythmic movement for gathering quests
- **Combat** - Fast, aggressive rotations for combat quests  
- **Explore** - Smooth, searching movements for exploration
- **Craft** - Precise, crafting-like rotations
- **Escort** - Steady, protective movements

#### **Click States (Triggers):**
- **IdleClick** - Quick spin when no quest active
- **GatherClick** - Digging/harvesting motion
- **CombatClick** - Attack strike motion
- **ExploreClick** - Looking around motion
- **CraftClick** - Hammering/working motion
- **EscortClick** - Alert/defensive motion

### 4. **Animator Parameters**

#### **Triggers:**
```
Idle          - Switch to idle loop
Gather        - Switch to gathering loop  
Combat        - Switch to combat loop
Explore       - Switch to exploration loop
Craft         - Switch to crafting loop
Escort        - Switch to escort loop

IdleClick     - One-shot click when idle
GatherClick   - One-shot gathering action
CombatClick   - One-shot combat action
ExploreClick  - One-shot exploration action
CraftClick    - One-shot crafting action
EscortClick   - One-shot escort action
```

#### **Bool:**
```
HasActiveQuest - true when player has active quest
```

### 5. **Animation Creation Suggestions**

#### **Idle Animations:**
- **Idle**: Slow Y-axis rotation (360° in 3-4 seconds)
- **IdleClick**: Quick 90° rotation on Y-axis

#### **Gather Animations:**
- **Gather**: Gentle up/down bobbing + slow rotation
- **GatherClick**: Quick down-up movement (like digging)

#### **Combat Animations:**
- **Combat**: Fast aggressive rotation + slight scaling
- **CombatClick**: Sharp forward thrust motion

#### **Explore Animations:**
- **Explore**: Smooth figure-8 or orbital movement
- **ExploreClick**: Quick left-right scanning motion

#### **Craft Animations:**
- **Craft**: Rhythmic up-down motion (like hammering)
- **CraftClick**: Strong down motion (like striking)

#### **Escort Animations:**
- **Escort**: Steady rotation with periodic pauses
- **EscortClick**: Alert darting motion

### 6. **State Machine Setup**

#### **Entry State:**
- Default: `Idle`

#### **Transitions:**
```
Any State → Idle         (Trigger: Idle)
Any State → Gather       (Trigger: Gather)  
Any State → Combat       (Trigger: Combat)
Any State → Explore      (Trigger: Explore)
Any State → Craft        (Trigger: Craft)
Any State → Escort       (Trigger: Escort)

Each Loop State → Corresponding Click State (on trigger)
Each Click State → Back to Loop State (on animation end)
```

#### **Transition Settings:**
- **Has Exit Time**: false for all trigger transitions
- **Transition Duration**: 0.1-0.2 seconds
- **Interruption Source**: Current State

### 7. **Player.cs Configuration**

In the Unity Inspector, set up these trigger names in the Player component:

```
Idle Animation Trigger: "Idle"
Gather Animation Trigger: "Gather"  
Combat Animation Trigger: "Combat"
Explore Animation Trigger: "Explore"
Craft Animation Trigger: "Craft"
Escort Animation Trigger: "Escort"
```

### 8. **Animation Clips Creation**

#### **Using Unity Animation Window:**
1. Select PlayerCube in scene
2. Open Window → Animation → Animation
3. Create new animation clips for each state
4. Animate Transform properties:
   - Position (for bobbing/movement)
   - Rotation (for spinning/turning)
   - Scale (for emphasis effects)

#### **Complete Animation Clip Examples:**

---

## 🔄 **BASE LOOP ANIMATIONS**

### **1. Idle Animation (4 seconds, looping)**
**Purpose**: Gentle, slow rotation when no quest is active
```
Duration: 4.0 seconds
Loop: Yes

Keyframes:
Frame 0 (0.0s):    Rotation Y = 0°
Frame 120 (4.0s):  Rotation Y = 360°

Curve Type: Linear
Ease: None (constant speed)
```

### **2. Gather Animation (3 seconds, looping)**
**Purpose**: Rhythmic up/down movement with slow rotation (like digging/harvesting)
```
Duration: 3.0 seconds
Loop: Yes

--- Position Y Track ---
Frame 0 (0.0s):    Position Y = 0
Frame 22 (0.75s):  Position Y = -0.15
Frame 45 (1.5s):   Position Y = 0
Frame 67 (2.25s):  Position Y = -0.15
Frame 90 (3.0s):   Position Y = 0

--- Rotation Y Track ---
Frame 0 (0.0s):    Rotation Y = 0°
Frame 90 (3.0s):   Rotation Y = 180°

--- Scale Track ---
Frame 0 (0.0s):    Scale = (1, 1, 1)
Frame 22 (0.75s):  Scale = (1, 0.9, 1)
Frame 45 (1.5s):   Scale = (1, 1, 1)
Frame 67 (2.25s):  Scale = (1, 0.9, 1)
Frame 90 (3.0s):   Scale = (1, 1, 1)

Curve Type: Ease In/Out for Position and Scale
```

### **3. Combat Animation (2 seconds, looping)**
**Purpose**: Fast, aggressive movements with scaling (like battle ready)
```
Duration: 2.0 seconds
Loop: Yes

--- Rotation Y Track ---
Frame 0 (0.0s):    Rotation Y = 0°
Frame 60 (2.0s):   Rotation Y = 720° (double spin)

--- Rotation Z Track ---
Frame 0 (0.0s):    Rotation Z = 0°
Frame 15 (0.5s):   Rotation Z = 15°
Frame 30 (1.0s):   Rotation Z = -15°
Frame 45 (1.5s):   Rotation Z = 15°
Frame 60 (2.0s):   Rotation Z = 0°

--- Scale Track ---
Frame 0 (0.0s):    Scale = (1, 1, 1)
Frame 15 (0.5s):   Scale = (1.1, 1.1, 1.1)
Frame 30 (1.0s):   Scale = (1, 1, 1)
Frame 45 (1.5s):   Scale = (1.1, 1.1, 1.1)
Frame 60 (2.0s):   Scale = (1, 1, 1)

Curve Type: Ease In/Out for all tracks
```

### **4. Explore Animation (5 seconds, looping)**
**Purpose**: Smooth, searching movements (like scanning environment)
```
Duration: 5.0 seconds
Loop: Yes

--- Position X Track ---
Frame 0 (0.0s):    Position X = 0
Frame 37 (1.25s):  Position X = 0.3
Frame 75 (2.5s):   Position X = 0
Frame 112 (3.75s): Position X = -0.3
Frame 150 (5.0s):  Position X = 0

--- Position Z Track ---
Frame 0 (0.0s):    Position Z = 0
Frame 18 (0.6s):   Position Z = 0.2
Frame 37 (1.25s):  Position Z = 0
Frame 56 (1.9s):   Position Z = -0.2
Frame 75 (2.5s):   Position Z = 0
Frame 93 (3.1s):   Position Z = 0.2
Frame 112 (3.75s): Position Z = 0
Frame 131 (4.4s):  Position Z = -0.2
Frame 150 (5.0s):  Position Z = 0

--- Rotation Y Track ---
Frame 0 (0.0s):    Rotation Y = 0°
Frame 150 (5.0s):  Rotation Y = 360°

Curve Type: Smooth (figure-8 movement)
```

### **5. Craft Animation (2.5 seconds, looping)**
**Purpose**: Rhythmic hammering/working motion
```
Duration: 2.5 seconds
Loop: Yes

--- Position Y Track ---
Frame 0 (0.0s):    Position Y = 0
Frame 18 (0.6s):   Position Y = 0.25
Frame 25 (0.8s):   Position Y = -0.1
Frame 31 (1.0s):   Position Y = 0
Frame 50 (1.6s):   Position Y = 0.25
Frame 56 (1.8s):   Position Y = -0.1
Frame 62 (2.0s):   Position Y = 0
Frame 75 (2.5s):   Position Y = 0

--- Scale Y Track ---
Frame 0 (0.0s):    Scale Y = 1
Frame 25 (0.8s):   Scale Y = 0.8
Frame 31 (1.0s):   Scale Y = 1
Frame 56 (1.8s):   Scale Y = 0.8
Frame 62 (2.0s):   Scale Y = 1
Frame 75 (2.5s):   Scale Y = 1

--- Rotation Y Track ---
Frame 0 (0.0s):    Rotation Y = 0°
Frame 75 (2.5s):   Rotation Y = 90°

Curve Type: Sharp for Position Y (hammering), Linear for Rotation
```

### **6. Escort Animation (4 seconds, looping)**
**Purpose**: Steady, protective movement with periodic pauses
```
Duration: 4.0 seconds
Loop: Yes

--- Rotation Y Track ---
Frame 0 (0.0s):    Rotation Y = 0°
Frame 30 (1.0s):   Rotation Y = 90°
Frame 40 (1.33s):  Rotation Y = 90° (pause)
Frame 70 (2.33s):  Rotation Y = 180°
Frame 80 (2.67s):  Rotation Y = 180° (pause)
Frame 110 (3.67s): Rotation Y = 270°
Frame 120 (4.0s):  Rotation Y = 360°

--- Scale Track ---
Frame 30 (1.0s):   Scale = (1.05, 1.05, 1.05) (alert)
Frame 40 (1.33s):  Scale = (1, 1, 1)
Frame 80 (2.67s):  Scale = (1.05, 1.05, 1.05) (alert)
Frame 90 (3.0s):   Scale = (1, 1, 1)

Curve Type: Step for pauses, Ease for scaling
```

---

## ⚡ **CLICK ANIMATIONS (One-shot)**

### **1. IdleClick Animation (0.4 seconds)**
**Purpose**: Quick spin response when no quest active
```
Duration: 0.4 seconds
Loop: No

Keyframes:
Frame 0 (0.0s):   Rotation Y = 0°, Scale = (1, 1, 1)
Frame 6 (0.2s):   Rotation Y = 180°, Scale = (1.2, 1.2, 1.2)
Frame 12 (0.4s):  Rotation Y = 360°, Scale = (1, 1, 1)

Curve Type: Ease Out
```

### **2. GatherClick Animation (0.6 seconds)**
**Purpose**: Digging/harvesting motion
```
Duration: 0.6 seconds
Loop: No

--- Position Y Track ---
Frame 0 (0.0s):   Position Y = 0
Frame 9 (0.3s):   Position Y = -0.3
Frame 18 (0.6s):  Position Y = 0

--- Scale Track ---
Frame 0 (0.0s):   Scale = (1, 1, 1)
Frame 9 (0.3s):   Scale = (1.1, 0.7, 1.1)
Frame 18 (0.6s):  Scale = (1, 1, 1)

--- Rotation X Track ---
Frame 0 (0.0s):   Rotation X = 0°
Frame 9 (0.3s):   Rotation X = 25°
Frame 18 (0.6s):  Rotation X = 0°

Curve Type: Ease In/Out
```

### **3. CombatClick Animation (0.3 seconds)**
**Purpose**: Sharp attack/strike motion
```
Duration: 0.3 seconds
Loop: No

--- Position Z Track ---
Frame 0 (0.0s):   Position Z = 0
Frame 4 (0.13s):  Position Z = 0.4
Frame 9 (0.3s):   Position Z = 0

--- Scale Track ---
Frame 0 (0.0s):   Scale = (1, 1, 1)
Frame 4 (0.13s):  Scale = (1.3, 1.3, 1.3)
Frame 9 (0.3s):   Scale = (1, 1, 1)

--- Rotation Z Track ---
Frame 0 (0.0s):   Rotation Z = 0°
Frame 4 (0.13s):  Rotation Z = 45°
Frame 9 (0.3s):   Rotation Z = 0°

Curve Type: Sharp attack motion
```

### **4. ExploreClick Animation (0.5 seconds)**
**Purpose**: Quick scanning/searching motion
```
Duration: 0.5 seconds
Loop: No

--- Rotation Y Track ---
Frame 0 (0.0s):   Rotation Y = 0°
Frame 7 (0.25s):  Rotation Y = -45°
Frame 15 (0.5s):  Rotation Y = 45°

--- Position Y Track ---
Frame 0 (0.0s):   Position Y = 0
Frame 7 (0.25s):  Position Y = 0.1
Frame 15 (0.5s):  Position Y = 0

--- Scale Track ---
Frame 7 (0.25s):  Scale = (1.1, 1.1, 1.1)

Curve Type: Quick scanning motion
```

### **5. CraftClick Animation (0.4 seconds)**
**Purpose**: Strong hammering/crafting strike
```
Duration: 0.4 seconds
Loop: No

--- Position Y Track ---
Frame 0 (0.0s):   Position Y = 0
Frame 6 (0.2s):   Position Y = 0.3
Frame 8 (0.27s):  Position Y = -0.2
Frame 12 (0.4s):  Position Y = 0

--- Scale Y Track ---
Frame 0 (0.0s):   Scale Y = 1
Frame 8 (0.27s):  Scale Y = 0.6
Frame 12 (0.4s):  Scale Y = 1

--- Rotation Z Track ---
Frame 6 (0.2s):   Rotation Z = -20°
Frame 8 (0.27s):  Rotation Z = 20°
Frame 12 (0.4s):  Rotation Z = 0°

Curve Type: Sharp impact motion
```

### **6. EscortClick Animation (0.35 seconds)**
**Purpose**: Alert/defensive response
```
Duration: 0.35 seconds
Loop: No

--- Scale Track ---
Frame 0 (0.0s):   Scale = (1, 1, 1)
Frame 5 (0.17s):  Scale = (1.2, 1.2, 1.2)
Frame 10 (0.35s): Scale = (1, 1, 1)

--- Rotation Y Track ---
Frame 0 (0.0s):   Rotation Y = 0°
Frame 5 (0.17s):  Rotation Y = 90°
Frame 10 (0.35s): Rotation Y = 0°

--- Position Y Track ---
Frame 5 (0.17s):  Position Y = 0.1

Curve Type: Alert response
```

---

## 📐 **Animation Creation Steps:**

### **For Each Animation:**
1. **Create New Clip**: Animation window → Create → [AnimationName]
2. **Set Duration**: Adjust timeline length
3. **Add Properties**: Click "Add Property" → Transform
4. **Set Keyframes**: Click diamond icons to add keyframes
5. **Adjust Curves**: Right-click keyframes → set interpolation
6. **Preview**: Use play button to test animation
7. **Loop Settings**: Inspector → Loop Time (for base animations)

### **Curve Types Guide:**
- **Linear**: Constant speed, mechanical movement
- **Ease In/Out**: Natural, organic movement
- **Sharp**: Sudden impacts, strikes
- **Step**: Pauses, robotic movement
- **Smooth**: Flowing, graceful movement

### 9. **Testing the System**

1. **Start Scene**: PlayerCube should begin with Idle animation
2. **Get Quest**: Animation should change based on quest type
3. **Click**: Should trigger quest-specific click animation
4. **Complete Quest**: Should return to Idle animation

### 10. **Debug Console Output**

When working correctly, you should see:
```
🎯 Player quest type set to: Gather
🎬 Cube animation changed to: Gather (Quest Active: True)
🏁 Player quest cleared, returned to idle state
🎬 Cube animation changed to: Idle (Quest Active: False)
```

## 🎨 Visual Polish Tips

### **Color Coding:**
- Add materials/colors that change with quest types
- Use Unity's Timeline or Animation Events for color changes

### **Particle Effects:**
- Add particle systems that activate with different quest types
- Combat: Sparks or flames
- Gather: Dust or leaves
- Explore: Glowing orbs
- Craft: Tool sparkles
- Escort: Shield particles

### **Sound Integration:**
- Use Animation Events to trigger different sound effects
- Each quest type can have unique audio feedback

## 🔧 Troubleshooting

### **Common Issues:**

1. **Animations not triggering:**
   - Check trigger names match exactly in code and Animator
   - Verify PlayerCube has Animator component
   - Ensure transitions exist between states

2. **Animations not returning to base state:**
   - Check "Has Exit Time" is properly set
   - Verify click animations transition back to loop states

3. **Player.cs can't find PlayerCube:**
   - Ensure GameObject is named exactly "PlayerCube"
   - Check the object is active in scene hierarchy

4. **Quest state not updating:**
   - Verify QuestManager calls SetCurrentQuestType()
   - Check Player.instance is not null
   - Ensure ClearCurrentQuest() is called on completion

## 🎯 Result
After setup, your PlayerCube will:
- ✅ Rotate gently when idle (no active quest)
- ✅ Change animation style based on quest type
- ✅ Perform unique click animations for each quest type
- ✅ Return to idle when quest completes
- ✅ Provide visual feedback for different gameplay states

This creates an engaging visual system that gives players immediate feedback about their current quest type and progress!
