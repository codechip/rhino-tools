using System;

namespace SolidDemo
{
    public interface ICellularPhone
    {
        void Call(string number);
        void Play(string file);
        void Send(string sms);
    }

    public class NewPhone : ICellularPhone
    {
        public void Call(string number)
        {
            Console.WriteLine("calling: "+number);
        }

        public void Play(string file)
        {
            Console.WriteLine("Playing " + file);
        }

        public void Send(string sms)
        {
            Console.WriteLine("sending: " + sms);
        }
    }

    public class StandardPhone : ICellularPhone
    {
        public void Call(string number)
        {
            Console.WriteLine("calling: " + number);
        }

        public void Play(string file)
        {
            throw new System.NotImplementedException("I am am a business device, not an entertainment device");
        }

        public void Send(string sms)
        {
            Console.WriteLine("sending: " + sms);
        }
    }

    public class OldPhone : ICellularPhone
    {
        public void Call(string number)
        {
            Console.WriteLine("calling: " + number);
        }

        public void Play(string file)
        {
            throw new System.NotImplementedException("Huh? speak up, can't hear you");
        }

        public void Send(string sms)
        {
            throw new System.NotImplementedException(
                "What do you want ME to do now? Just send a letter and be done with it");
        }
    }
}