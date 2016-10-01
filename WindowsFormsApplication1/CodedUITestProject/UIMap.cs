namespace CodedUITestProject
{
    using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
    using System;
    using System.Collections.Generic;
    using System.CodeDom.Compiler;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = System.Windows.Forms.MouseButtons;
    using System.Drawing;
    using System.Windows.Input;
    using System.Text.RegularExpressions;


    public partial class UIMap
    {
        // http://stackoverflow.com/questions/19670364/why-is-it-bad-to-edit-the-uimap-designer-cs-file-in-a-visual-studio-coded-ui-tes

        /// <summary>
        /// AssertCheckDatabaseProgressTable - Use 'AssertCheckDatabaseProgressTableExpectedValues' to pass parameters into this method.
        /// </summary>
        public void AssertCheckDatabaseProgressTable()
        {
            #region Variable Declarations
            WinClient uIDatabase_TableLayoutClient = this.UIDatabaseEvaluatorWindow.UIDatabase_TableLayoutWindow.UIDatabase_TableLayoutClient;
            #endregion

            // Verify that the 'Enabled' property of 'Database_TableLayoutPanel' client equals 'True'
            Assert.AreEqual(this.AssertCheckDatabaseProgressTableExpectedValues.UIDatabase_TableLayoutClientEnabled, uIDatabase_TableLayoutClient.Enabled);

            // Click 'Close' button
            WinButton uICloseButton = this.UIDatabaseEvaluatorWindow.UICloseWindow.UICloseButton;
            Mouse.Click(uICloseButton, new Point(45, 19));
        }

        public virtual AssertCheckDatabaseProgressTableExpectedValues AssertCheckDatabaseProgressTableExpectedValues
        {
            get
            {
                if ((this.mAssertCheckDatabaseProgressTableExpectedValues == null))
                {
                    this.mAssertCheckDatabaseProgressTableExpectedValues = new AssertCheckDatabaseProgressTableExpectedValues();
                }
                return this.mAssertCheckDatabaseProgressTableExpectedValues;
            }
        }

        private AssertCheckDatabaseProgressTableExpectedValues mAssertCheckDatabaseProgressTableExpectedValues;

        /// <summary>
        /// AssertDropdownValue - Use 'AssertDropdownValueExpectedValues' to pass parameters into this method.
        /// </summary>
        public void AssertDropdownValue()
        {
            #region Variable Declarations
            WinEdit uIItemEdit = this.UIDatabaseEvaluatorWindow.UIItemWindow.UIItemEdit;
            #endregion

            // Verify that the 'Text' property of text box equals 'DESKTOP-FVFO8GL\SQL2016N'
            Assert.AreEqual(this.AssertDropdownValueExpectedValues.UIItemEditText, uIItemEdit.Text);

            // Click 'Close' button
            WinButton uICloseButton = this.UIDatabaseEvaluatorWindow.UICloseWindow.UICloseButton;
            Mouse.Click(uICloseButton, new Point(45, 19));
        }

        public virtual AssertDropdownValueExpectedValues AssertDropdownValueExpectedValues
        {
            get
            {
                if ((this.mAssertDropdownValueExpectedValues == null))
                {
                    this.mAssertDropdownValueExpectedValues = new AssertDropdownValueExpectedValues();
                }
                return this.mAssertDropdownValueExpectedValues;
            }
        }

        private AssertDropdownValueExpectedValues mAssertDropdownValueExpectedValues;
    }
    /// <summary>
    /// Parameters to be passed into 'AssertCheckDatabaseProgressTable'
    /// </summary>
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class AssertCheckDatabaseProgressTableExpectedValues
    {

        #region Fields
        /// <summary>
        /// Verify that the 'Enabled' property of 'Database_TableLayoutPanel' client equals 'True'
        /// </summary>
        public bool UIDatabase_TableLayoutClientEnabled = true;
        #endregion
    }
    /// <summary>
    /// Parameters to be passed into 'AssertDropdownValue'
    /// </summary>
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class AssertDropdownValueExpectedValues
    {

        #region Fields
        /// <summary>
        /// Verify that the 'Text' property of text box equals 'DESKTOP-FVFO8GL\SQL2016N'
        /// </summary>
        public string UIItemEditText = "DESKTOP-FVFO8GL\\SQL2016N";
        #endregion
    }
}
