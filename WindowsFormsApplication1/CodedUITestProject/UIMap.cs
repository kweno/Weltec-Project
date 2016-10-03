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
            WinCheckBox uIDatabaseNameCheckBox = this.UIDatabaseEvaluatorWindow.UIDatabaseNameWindow.UIDatabaseNameCheckBox;
            #endregion

            // Verify that the 'Text' property of text box equals 'DESKTOP-FVFO8GL\SQL2016N'
            Playback.Wait(2000);
            //Assert.AreEqual(this.AssertDropdownValueExpectedValues.UIItemEditText, uIItemEdit.Text);
            Assert.AreEqual(true, uIDatabaseNameCheckBox.Enabled);


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

        /// <summary>
        /// CheckDatabaseName - Use 'CheckDatabaseNameParams' to pass parameters into this method.
        /// </summary>
        public void CheckDatabaseName()
        {
            #region Variable Declarations
            WinEdit uINameEdit = this.UIReleaseWindow.UIItemWindow.UIClientApplicationexeListItem.UINameEdit;
            WinComboBox uIServer_ComboBoxComboBox = this.UIDatabaseEvaluatorWindow.UIServer_ComboBoxWindow.UIServer_ComboBoxComboBox;
            WinCheckBox uIDatabaseNameCheckBox = this.UIDatabaseEvaluatorWindow.UIDatabaseNameWindow.UIDatabaseNameCheckBox;
            #endregion

            // Double-Click 'Name' text box
            Mouse.DoubleClick(uINameEdit, new Point(105, 6));

            // Wait for 30 seconds for user delay between actions; Select 'DESKTOP-FVFO8GL\SQL2016N' in 'Server_ComboBox' combo box
            Playback.Wait(30000);
            uIServer_ComboBoxComboBox.SelectedIndex = 0;
            //uIServer_ComboBoxComboBox.SelectedItem = this.SelectItemFromDropdownParams.UIServer_ComboBoxComboBoxSelectedItem;

            // Wait for 5 seconds for user delay between actions; Select 'Database Name:' check box
            Playback.Wait(5000);
            uIDatabaseNameCheckBox.Checked = this.CheckDatabaseNameParams.UIDatabaseNameCheckBoxChecked;
        }

        public virtual CheckDatabaseNameParams CheckDatabaseNameParams
        {
            get
            {
                if ((this.mCheckDatabaseNameParams == null))
                {
                    this.mCheckDatabaseNameParams = new CheckDatabaseNameParams();
                }
                return this.mCheckDatabaseNameParams;
            }
        }

        private CheckDatabaseNameParams mCheckDatabaseNameParams;

        /// <summary>
        /// SelectItemFromDropdown - Use 'SelectItemFromDropdownParams' to pass parameters into this method.
        /// </summary>
        public void SelectItemFromDropdown()
        {
            #region Variable Declarations
            WinEdit uINameEdit = this.UIReleaseWindow.UIItemWindow.UIClientApplicationexeListItem.UINameEdit;
            WinComboBox uIServer_ComboBoxComboBox = this.UIDatabaseEvaluatorWindow.UIServer_ComboBoxWindow.UIServer_ComboBoxComboBox;
            #endregion

            // Double-Click 'Name' text box
            Mouse.DoubleClick(uINameEdit, new Point(105, 11));

            // Wait for 30 seconds for user delay between actions; Select 'DESKTOP-FVFO8GL\SQL2016N' in 'Server_ComboBox' combo box
            Playback.Wait(30000);
            //uIServer_ComboBoxComboBox.SelectedItem = this.SelectItemFromDropdownParams.UIServer_ComboBoxComboBoxSelectedItem;
            uIServer_ComboBoxComboBox.SelectedIndex = 0;
        }

        public virtual SelectItemFromDropdownParams SelectItemFromDropdownParams
        {
            get
            {
                if ((this.mSelectItemFromDropdownParams == null))
                {
                    this.mSelectItemFromDropdownParams = new SelectItemFromDropdownParams();
                }
                return this.mSelectItemFromDropdownParams;
            }
        }

        private SelectItemFromDropdownParams mSelectItemFromDropdownParams;
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
    /// <summary>
    /// Parameters to be passed into 'CheckDatabaseName'
    /// </summary>
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class CheckDatabaseNameParams
    {

        #region Fields
        /// <summary>
        /// Wait for 30 seconds for user delay between actions; Select 'DESKTOP-FVFO8GL\SQL2016N' in 'Server_ComboBox' combo box
        /// </summary>
        public string UIServer_ComboBoxComboBoxSelectedItem = "DESKTOP-FVFO8GL\\SQL2016N";

        /// <summary>
        /// Wait for 5 seconds for user delay between actions; Select 'Database Name:' check box
        /// </summary>
        public bool UIDatabaseNameCheckBoxChecked = true;
        #endregion
    }
    /// <summary>
    /// Parameters to be passed into 'SelectItemFromDropdown'
    /// </summary>
    [GeneratedCode("Coded UITest Builder", "14.0.23107.0")]
    public class SelectItemFromDropdownParams
    {

        #region Fields
        /// <summary>
        /// Wait for 30 seconds for user delay between actions; Select 'DESKTOP-FVFO8GL\SQL2016N' in 'Server_ComboBox' combo box
        /// </summary>
        public string UIServer_ComboBoxComboBoxSelectedItem = "DESKTOP-FVFO8GL\\SQL2016N";
        #endregion
    }
}
