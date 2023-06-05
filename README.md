# NinjaFootIkRig
Unity TPS controller with Ninja demo and physics based foot IK and hip placements (Unity Animation Rigging)


Project includes all of the Ninja rig from Unity Ninja rig demo (workshop)


Foot ik is physics based: 
 - baked AnchoredPoints into animations
 - Rig Foot IK constaints and create IK Targets
 - use IK target with Rigidbidy and collider + SpringJoint
 - SpringJoint should be linked to animated AnchoredPoints

Issues left : 
 - Still need to fix sliding of the foot (partially solved, by using FootSolver distance prev positions)
 - Need properly placed physical colliders on the IK Target (done)

Included: 
 - slightly modified Unity TPS controller ( new one with New Input system)
 - FootSolver (solves foot rotation) with Hip target placement

Demo:

![FootIk](https://github.com/studentutu/NinjaFootIkRig/assets/18601652/fe3d37d3-505d-4bb0-9261-9798a983a46b)
