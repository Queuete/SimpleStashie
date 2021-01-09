﻿using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ExileCore.Shared.Attributes;
using System.Windows.Forms;

namespace SimpleStashie
{
    public class SimpleStashieSettings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(true);

        [Menu("Stash Inventory Hotkey")]
        public HotkeyNode StashItKey { get; set; } = new HotkeyNode(Keys.F2);

        [Menu("Time between Clicks in Milliseconds")]
        public RangeNode<int> WaitTimeInMs { get; set; } = new RangeNode<int>(75, 40, 200);

        [Menu("Amount of Retries")]
        public RangeNode<int> AmountOfRetries { get; set; } = new RangeNode<int>(3, 1, 5);

        [Menu("Inventory cells which should NOT be dumped to Stash")]
        public int[,] IgnoredCells { get; set; } =
        {
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        };
    }
}