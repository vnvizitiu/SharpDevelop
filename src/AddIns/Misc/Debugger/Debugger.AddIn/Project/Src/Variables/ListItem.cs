// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
#region License
//  
//  Copyright (c) 2007, ic#code
//  
//  All rights reserved.
//  
//  Redistribution  and  use  in  source  and  binary  forms,  with  or without
//  modification, are permitted provided that the following conditions are met:
//  
//  1. Redistributions  of  source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//  
//  2. Redistributions  in  binary  form  must  reproduce  the  above copyright
//     notice,  this  list  of  conditions  and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//  
//  3. Neither the name of the ic#code nor the names of its contributors may be
//     used  to  endorse or promote products derived from this software without
//     specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND  ANY  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE  DISCLAIMED.   IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//  LIABLE  FOR  ANY  DIRECT,  INDIRECT,  INCIDENTAL,  SPECIAL,  EXEMPLARY,  OR
//  CONSEQUENTIAL  DAMAGES  (INCLUDING,  BUT  NOT  LIMITED  TO,  PROCUREMENT OF
//  SUBSTITUTE  GOODS  OR  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION)  HOWEVER  CAUSED  AND  ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT,  STRICT  LIABILITY,  OR  TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING  IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//  
#endregion

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Debugger
{
	public abstract class ListItem
	{
		public event EventHandler<ListItemEventArgs> Changed;
		
		public abstract int ImageIndex { get; }
		public abstract string Name { get; }
		public abstract string Text { get; }
		public abstract bool CanEditText { get; }
		public abstract string Type  { get; }
		public abstract bool HasSubItems  { get; }
		public abstract IList<ListItem> SubItems { get; }
		
		public System.Drawing.Image Image {
			get {
				if (ImageIndex == -1) {
					return null;
				} else {
					return DebuggerIcons.ImageList.Images[ImageIndex];
				}
			}
		}
		
		protected virtual void OnChanged(ListItemEventArgs e)
		{
			if (Changed != null) {
				Changed(this, e);
			}
		}
		
		public virtual bool SetText(string newValue)
		{
			throw new NotImplementedException();
		}
		
		public virtual ContextMenuStrip GetContextMenu()
		{
			return null;
		}
	}
}
