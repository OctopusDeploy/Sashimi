using System;
using System.Collections.Generic;
using System.Linq;

namespace Sashimi.Server.Contracts.Variables
{
    public class WellKnownVariable
    {
        public WellKnownVariable(string name, Visibility visibility = Visibility.Hidden, params string[] aliases)
        {
            Name = name;
            Aliases = aliases;
            Visibility = visibility;
        }

        public string Name { get; }
        public IEnumerable<string> Aliases { get; }
        public Visibility Visibility { get; }

        protected bool Equals(WellKnownVariable other)
        {
            return Name == other.Name
                   && Aliases.SequenceEqual(other.Aliases) //This is stricter than it needs to be but we're unlikely to even have more than 1 alias
                   && Visibility == other.Visibility;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((WellKnownVariable)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Aliases, (int)Visibility);
        }

        public static bool operator ==(WellKnownVariable? left, WellKnownVariable? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WellKnownVariable? left, WellKnownVariable? right)
        {
            return !Equals(left, right);
        }

        public static implicit operator string?(WellKnownVariable? wellKnownVariable)
        {
            return wellKnownVariable?.Name;
        }

        //Override ToString() because these used to be constants, so we want to retain this behaviour in case we are constructing a string somewhere
        public override string ToString()
        {
            return Name;
        }
    }

    public enum Visibility
    {
        Visible,
        Hidden
    }
}