using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIBirthdayForm : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown year;
        [SerializeField] private TMP_Dropdown month;
        [SerializeField] private TMP_Dropdown day;

        private const int startingYear = 1900;

        public int Year => year.value + startingYear;
        public int Month => month.value + 1;
        public int Day => day.value + 1;
        public DateTime Date => new(Year, Month, Day);

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            InitializeYears();
            InitializeMonth();
            InitializeDays();
        }

        private void InitializeYears()
        {
            for (int i = startingYear; i <= DateTime.Now.Year; i++)
            {
                year.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
            }
            year.value = 0;
        }

        private void InitializeMonth()
        {
            month.options.Add(new TMP_Dropdown.OptionData("January"));
            month.options.Add(new TMP_Dropdown.OptionData("February"));
            month.options.Add(new TMP_Dropdown.OptionData("March"));
            month.options.Add(new TMP_Dropdown.OptionData("April"));
            month.options.Add(new TMP_Dropdown.OptionData("May"));
            month.options.Add(new TMP_Dropdown.OptionData("June"));
            month.options.Add(new TMP_Dropdown.OptionData("July"));
            month.options.Add(new TMP_Dropdown.OptionData("August"));
            month.options.Add(new TMP_Dropdown.OptionData("September"));
            month.options.Add(new TMP_Dropdown.OptionData("October"));
            month.options.Add(new TMP_Dropdown.OptionData("November"));
            month.options.Add(new TMP_Dropdown.OptionData("December"));
        }

        public void InitializeDays()
        {
            switch (month.value + 1)
            {
                case 2:
                    SetDaysForFebruary();
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    SetDays(30);
                    break;
                default:
                    SetDays(31);
                    break;
            }
        }

        private void SetDaysForFebruary()
        {
            if (DateTime.IsLeapYear(year.value + startingYear))
            {
                SetDays(29);
            }
            else
            {
                SetDays(28);
            }
        }

        private void SetDays(int lastDay)
        {
            for (int i = 1; i <= lastDay; i++)
            {
                day.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
            }
        }
    }
}
