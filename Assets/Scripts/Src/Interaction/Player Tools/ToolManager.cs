using Fusion;
using game.assets.interaction;
using game.assets.tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game.assets.player
{
    public class ToolManager : NetworkBehaviour
    {
        private const float BUFFER_BETWEEN_PRESSES = 0.2f;

        private float lastPressed;

        public BuilderTool builderTool;
        public CommandTool commandTool;

        public override void FixedUpdateNetwork()
        {
            commandTool.leftClickEnabled = !builderTool.placingBuilding;
            if (Object.HasStateAuthority && GetInput(out PlayerNetworkInput input))
            {
                if (input.IsDown(PlayerNetworkInput.BUTTON_ACTION2))
                {
                    float lastPress = Time.time - lastPressed;

                    if (lastPress < BUFFER_BETWEEN_PRESSES)
                    {
                        return;
                    }

                    lastPressed = Time.time;

                    if (builderTool.placingBuilding)
                    {
                        builderTool.resetSpawner();
                    }
                    else
                    {
                        commandTool.clearSelection();
                    }
                }
            }
        }
    }
}
