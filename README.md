# Spine Synchronator
Simple GUI application for syncing Spine JSON file.

## Use Case

If you have multiple skin based on one skeleton, and want to optimize the asset atlas by using one skeleton for each skin. 

To sum up, we usually have:
- One skeleton for master animation and bones
- One skeleton for each skins with NO animation

Using this workflow, we can develop the animation on only one skeleton and use that to other skins. We do this by copying the animation/bone data from the exported master JSON to the other exported JSON files.
