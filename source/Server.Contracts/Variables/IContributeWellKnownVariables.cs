using System;
using System.Collections.Generic;
using System.Linq;

namespace Sashimi.Server.Contracts.Variables
{
    public interface IContributeWellKnownVariables
    {
        //IEnumerable<WellKnownVariable> GetWellKnownVariables();
        IEnumerable<string> GetUserVisibleWellKnownVariables();
        IEnumerable<WellKnownVariableAliasMapping> GetVariablesWithAliases();
    }

    public class WellKnownVariableAliasMapping
    {
        public WellKnownVariableAliasMapping(string currentName, params string[] aliases)
        {
            CurrentName = currentName;
            Aliases = aliases;
        }

        public string CurrentName { get; }
        public IEnumerable<string> Aliases { get; }

        protected bool Equals(WellKnownVariableAliasMapping other)
        {
            return CurrentName == other.CurrentName && Aliases.SequenceEqual(other.Aliases); //Strict
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((WellKnownVariableAliasMapping) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CurrentName, Aliases);
        }

        public static bool operator ==(WellKnownVariableAliasMapping? left, WellKnownVariableAliasMapping? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WellKnownVariableAliasMapping? left, WellKnownVariableAliasMapping? right)
        {
            return !Equals(left, right);
        }
    }
}