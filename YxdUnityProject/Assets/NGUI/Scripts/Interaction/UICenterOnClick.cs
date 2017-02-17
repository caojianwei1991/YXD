//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------
using UnityEngine;

/// <summary>
/// Attaching this script to an element of a scroll view will make it possible to center on it by clicking on it.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Center Scroll View on Click")]
public class UICenterOnClick : MonoBehaviour
{
	void OnClick ()
	{
		UICenterOnChild center = NGUITools.FindInParents<UICenterOnChild> (gameObject);
		UIPanel panel = NGUITools.FindInParents<UIPanel> (gameObject);

		if (center != null)
		{
			if (center.enabled)
			{
				center.CenterOn (transform);
			}
		}
		else if (panel != null && panel.clipping != UIDrawCall.Clipping.None)
		{
			UIScrollView sv = panel.GetComponent<UIScrollView> ();
			Vector3 offset = -panel.cachedTransform.InverseTransformPoint (transform.position);
			if (!sv.canMoveHorizontally)
			{
				offset.x = panel.cachedTransform.localPosition.x;
			}
			if (!sv.canMoveVertically)
			{
				offset.y = panel.cachedTransform.localPosition.y;
			}
			SpringPanel.Begin (panel.cachedGameObject, offset, 6f);
		}

		if (Mathf.Abs (localOffset.x) < 1)
		{
			LocalStorage.SelectWeek = weekIndex;
			PaperList.Show ();
		}
	}

	public int weekIndex;
	UIScrollView mScrollView;
	UIPanel mPanel;
	float width;
	Vector3 localOffset;

	void Start ()
	{
		mPanel = transform.parent.parent.GetComponent<UIPanel> ();
		mScrollView = mPanel.GetComponent<UIScrollView> ();
		width = transform.parent.GetComponent<UIWrapContent> ().itemSize;
	}

	void Update ()
	{
		Vector3[] corners = mPanel.worldCorners;
		Vector3 panelCenter = (corners [2] + corners [0]) * 0.5f;
		Vector3 cp = mPanel.cachedTransform.InverseTransformPoint (transform.position);
		Vector3 cc = mPanel.cachedTransform.InverseTransformPoint (panelCenter);
		localOffset = cp - cc;
		if (!mScrollView.canMoveHorizontally)
		{
			localOffset.x = 0f;
		}
		if (!mScrollView.canMoveVertically)
		{
			localOffset.y = 0f;
		}
		localOffset.z = 0f;

		Vector3 lp = transform.localPosition;
		lp.y = (-200.0f / width) * (width - Mathf.Abs (localOffset.x));
		transform.localPosition = lp;

		Vector3 ls = Vector3.one;
		ls *= ((0.4f / width) * (width - Mathf.Abs (localOffset.x))) + 0.6f;
		transform.localScale = ls;
	}
}
