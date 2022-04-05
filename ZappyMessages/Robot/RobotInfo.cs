namespace ZappyMessages.Robot
{
    public struct RobotInfo
    {
        public string Guid { get; set; }

        public string Name { get; set; }

        public string Machine { get; set; }

        public string UserName { get; set; }

        public ZappyRobotState State { get; set; }
    }
}
