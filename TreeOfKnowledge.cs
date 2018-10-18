using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeOfKnowledge
{
    public partial class TreeOfKnowledge : Form
    {
        TOKTree _tree;

        public TreeOfKnowledge()
        {
            InitializeComponent();

            newTree();
        }

        private void newTree()
        {
            treeView.Nodes.Clear();
            textView.Clear();

            _tree = new TOKTree("");
            TreeNode root = makeTreeNode(_tree.Root);
            treeView.Nodes.Add(root);
            treeView.SelectedNode = root;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newTree();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog
            {
                CheckPathExists = true,
                Filter = "Tree Of Knowledge files (*.tok)|*.tok",
                DefaultExt = "tok",
                FilterIndex = 1
            };

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                treeView.Nodes.Clear();
                textView.Clear();

                _tree = new TOKTree("");
                _tree.Open(openDlg.FileName);
                TreeNode root = makeTreeNode(_tree.Root);
                treeView.Nodes.Add(root);
                treeView.SelectedNode = root;
                treeView.ExpandAll();
            }
        }

        private TreeNode makeTreeNode(TOKNode tokNode)
        {
            TreeNode treeNode = new TreeNode(tokNode.Name);
            treeNode.Tag = tokNode;

            if (tokNode.GetType() != typeof(TOKLeaf))
            {
                TOKBranch tokBranch = (TOKBranch)tokNode;
                foreach (TOKNode childTokNode in tokBranch.Nodes)
                {
                    TreeNode childTreeNode = makeTreeNode(childTokNode);
                    treeNode.Nodes.Add(childTreeNode);
                }
            }

            return treeNode;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog
            {
                CheckPathExists = true,
                Filter = "Tree Of Knowledge files (*.tok)|*.tok",
                DefaultExt = "tok",
                FilterIndex = 1
            };

            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                if (treeView.SelectedNode?.Tag?.GetType() == typeof(TOKLeaf))
                {
                    TOKLeaf leaf = (TOKLeaf)treeView.SelectedNode.Tag;
                    leaf.Text = String.Copy(textView.Text);
                }

                _tree.SaveAs(saveDlg.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void treeViewContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (treeView.SelectedNode.Tag?.GetType() == typeof(TOKRoot))
            {
                addBranchToolStripMenuItem.Enabled = true;
                addLeafToolStripMenuItem.Enabled = true;
                removeToolStripMenuItem.Enabled = false;
                renameToolStripMenuItem.Enabled = false;
            }
            else if (treeView.SelectedNode.Tag?.GetType() == typeof(TOKBranch))
            {
                addBranchToolStripMenuItem.Enabled = true;
                addLeafToolStripMenuItem.Enabled = true;
                removeToolStripMenuItem.Enabled = true;
                renameToolStripMenuItem.Enabled = true;
            }
            else if (treeView.SelectedNode.Tag?.GetType() == typeof(TOKLeaf))
            {
                addBranchToolStripMenuItem.Enabled = false;
                addLeafToolStripMenuItem.Enabled = false;
                removeToolStripMenuItem.Enabled = true;
                renameToolStripMenuItem.Enabled = true;
            }
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView.SelectedNode = e.Node;
        }

        private void addBranchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode.Tag?.GetType() != typeof(TOKLeaf))
            {
                TOKBranch branch = new TOKBranch();
                TOKBranch tokParent = (TOKBranch)treeView.SelectedNode.Tag;
                tokParent.Nodes.Add(branch);

                TreeNode treeNode = makeTreeNode(branch);
                treeView.SelectedNode.Nodes.Add(treeNode);
                treeView.SelectedNode = treeNode;
            }
        }

        private void addLeafToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode.Tag?.GetType() != typeof(TOKLeaf))
            {
                TOKLeaf leaf = new TOKLeaf();
                TOKBranch tokParent = (TOKBranch)treeView.SelectedNode.Tag;
                tokParent.Nodes.Add(leaf);

                TreeNode treeNode = makeTreeNode(leaf);
                treeView.SelectedNode.Nodes.Add(treeNode);
                treeView.SelectedNode = treeNode;
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode.Tag?.GetType() != typeof(TOKRoot))
            {
                TOKBranch tokParent = (TOKBranch)treeView.SelectedNode.Parent?.Tag;
                if (tokParent != null)
                {
                    tokParent.Nodes.Remove((TOKNode)treeView.SelectedNode.Tag);
                }

                treeView.SelectedNode.Remove();
            }
        }

        private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (treeView.SelectedNode?.Tag?.GetType() == typeof(TOKLeaf))
            {
                TOKLeaf leaf = (TOKLeaf)treeView.SelectedNode.Tag;
                leaf.Text = String.Copy(textView.Text);
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView.SelectedNode?.Tag?.GetType() == typeof(TOKLeaf))
            {
                TOKLeaf leaf = (TOKLeaf)treeView.SelectedNode.Tag;
                textView.Text = String.Copy(leaf.Text);
                textView.Enabled = true;
                textView.BackColor = SystemColors.Window;
            }
            else
            {
                textView.Text = "";
                textView.Enabled = false;
                textView.BackColor = SystemColors.ControlDark;
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode?.Tag?.GetType() != typeof(TOKRoot))
            {
                treeView.SelectedNode?.BeginEdit();
            }
        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                Regex alphaNumeric = new Regex("^[a-zA-Z0-9]*$");
                if (alphaNumeric.IsMatch(e.Label) == false)
                {
                    DialogResult result = MessageBox.Show(
                        "Invalid name given for node\r\n" +
                        "Use characters 'A-Z, a-z, or 0-9",
                        "Rename Node",
                        MessageBoxButtons.RetryCancel
                    );

                    e.CancelEdit = true;
                    if (result == DialogResult.Retry)
                    {
                        e.Node.BeginEdit();
                    }
                }
                else
                {
                    TOKNode tokNode = (TOKNode)e.Node.Tag;
                    tokNode.Name = String.Copy(e.Label);
                }
            }
            else
            {
                e.CancelEdit = true;
            }
        }
    }
}