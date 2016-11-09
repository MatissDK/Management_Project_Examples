namespace DelegateArgsLayer
{
    using System;
    using System.Collections.Generic;

    public class DelegateEventArgs : EventArgs
    {
        #region Constructors and Destructors

        public DelegateEventArgs(List<string> specializationStringList)
        {
            this.SpecializationStringList = specializationStringList;
        }

        public DelegateEventArgs(DateTime currentDate, bool onlySpecialists)
        {
            this.CurrentDate = currentDate;
            this.OnlySpecialists = onlySpecialists;
        }

        #endregion

        #region Public Properties

        public DateTime CurrentDate { get; set; }

        public bool OnlySpecialists { get; set; }

        public List<string> SpecializationStringList { get; set; }

        #endregion
    }
}