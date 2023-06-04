﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Live2DCSharpSDK.Framework.Rendering.OpenGL;

internal class CubismRendererProfile_OpenGLES2
{
    private OpenGLApi GL;
    /// <summary>
    /// モデル描画直前の頂点バッファ
    /// </summary>
    internal int _lastArrayBufferBinding;
    /// <summary>
    /// モデル描画直前のElementバッファ
    /// </summary>
    internal int _lastElementArrayBufferBinding;
    /// <summary>
    /// モデル描画直前のシェーダプログラムバッファ
    /// </summary>
    internal int _lastProgram;
    /// <summary>
    /// モデル描画直前のアクティブなテクスチャ
    /// </summary>
    internal int _lastActiveTexture;
    /// <summary>
    /// モデル描画直前のテクスチャユニット0
    /// </summary>
    internal int _lastTexture0Binding2D;
    /// <summary>
    /// モデル描画直前のテクスチャユニット1
    /// </summary>
    internal int _lastTexture1Binding2D;
    /// <summary>
    /// モデル描画直前のテクスチャユニット1
    /// </summary>
    internal int[] _lastVertexAttribArrayEnabled = new int[4];
    /// <summary>
    /// モデル描画直前のGL_VERTEX_ATTRIB_ARRAY_ENABLEDパラメータ
    /// </summary>
    internal bool _lastScissorTest;
    /// <summary>
    /// モデル描画直前のGL_SCISSOR_TESTパラメータ
    /// </summary>
    internal bool _lastBlend;
    /// <summary>
    /// モデル描画直前のGL_STENCIL_TESTパラメータ
    /// </summary>
    internal bool _lastStencilTest;
    /// <summary>
    /// モデル描画直前のGL_DEPTH_TESTパラメータ
    /// </summary>
    internal bool _lastDepthTest;
    /// <summary>
    /// モデル描画直前のGL_CULL_FACEパラメータ
    /// </summary>
    internal bool _lastCullFace;
    /// <summary>
    /// モデル描画直前のGL_CULL_FACEパラメータ
    /// </summary>
    internal int _lastFrontFace;
    /// <summary>
    /// モデル描画直前のGL_COLOR_WRITEMASKパラメータ
    /// </summary>
    internal bool[] _lastColorMask = new bool[4];
    /// <summary>
    /// モデル描画直前のカラーブレンディングパラメータ
    /// </summary>
    internal int[] _lastBlending = new int[4];
    /// <summary>
    /// モデル描画直前のフレームバッファ
    /// </summary>
    internal int _lastFBO;
    /// <summary>
    /// モデル描画直前のビューポート
    /// </summary>
    internal int[] _lastViewport = new int[4];

    public CubismRendererProfile_OpenGLES2(OpenGLApi gl)
    {
        GL = gl;
    }

    /// <summary>
    /// OpenGLES2のステートを保持する
    /// </summary>
    internal unsafe void Save()
    {
        //-- push state --
        fixed (int* ptr = &_lastArrayBufferBinding)
            GL.glGetIntegerv(GL.GL_ARRAY_BUFFER_BINDING, ptr);
        fixed (int* ptr = &_lastElementArrayBufferBinding)
            GL.glGetIntegerv(GL.GL_ELEMENT_ARRAY_BUFFER_BINDING, ptr);
        fixed (int* ptr = &_lastProgram)
            GL.glGetIntegerv(GL.GL_CURRENT_PROGRAM, ptr);

        fixed (int* ptr = &_lastActiveTexture)
            GL.glGetIntegerv(GL.GL_ACTIVE_TEXTURE, ptr);
        GL.glActiveTexture(GL.GL_TEXTURE1); //テクスチャユニット1をアクティブに（以後の設定対象とする）
        fixed (int* ptr = &_lastTexture1Binding2D)
            GL.glGetIntegerv(GL.GL_TEXTURE_BINDING_2D, ptr);

        GL.glActiveTexture(GL.GL_TEXTURE0); //テクスチャユニット0をアクティブに（以後の設定対象とする）
        fixed (int* ptr = &_lastTexture0Binding2D)
            GL.glGetIntegerv(GL.GL_TEXTURE_BINDING_2D, ptr);

        fixed (int* ptr = _lastVertexAttribArrayEnabled)
        {
            GL.glGetVertexAttribiv(0, GL.GL_VERTEX_ATTRIB_ARRAY_ENABLED, &ptr[0]);
            GL.glGetVertexAttribiv(1, GL.GL_VERTEX_ATTRIB_ARRAY_ENABLED, &ptr[1]);
            GL.glGetVertexAttribiv(2, GL.GL_VERTEX_ATTRIB_ARRAY_ENABLED, &ptr[2]);
            GL.glGetVertexAttribiv(3, GL.GL_VERTEX_ATTRIB_ARRAY_ENABLED, &ptr[3]);
        }

        _lastScissorTest = GL.glIsEnabled(GL.GL_SCISSOR_TEST);
        _lastStencilTest = GL.glIsEnabled(GL.GL_STENCIL_TEST);
        _lastDepthTest = GL.glIsEnabled(GL.GL_DEPTH_TEST);
        _lastCullFace = GL.glIsEnabled(GL.GL_CULL_FACE);
        _lastBlend = GL.glIsEnabled(GL.GL_BLEND);

        fixed (int* ptr = &_lastFrontFace)
            GL.glGetIntegerv(GL.GL_FRONT_FACE, ptr);

        GL.glGetBooleanv(GL.GL_COLOR_WRITEMASK, _lastColorMask);

        // backup blending
        fixed (int* ptr = _lastBlending)
        {
            GL.glGetIntegerv(GL.GL_BLEND_SRC_RGB, &ptr[0]);
            GL.glGetIntegerv(GL.GL_BLEND_DST_RGB, &ptr[1]);
            GL.glGetIntegerv(GL.GL_BLEND_SRC_ALPHA, &ptr[2]);
            GL.glGetIntegerv(GL.GL_BLEND_DST_ALPHA, &ptr[3]);
        }

        // モデル描画直前のFBOとビューポートを保存
        fixed (int* ptr = &_lastFBO)
            GL.glGetIntegerv(GL.GL_FRAMEBUFFER_BINDING, ptr);
        GL.glGetIntegerv(GL.GL_VIEWPORT, _lastViewport);
    }

    /// <summary>
    /// 保持したOpenGLES2のステートを復帰させる
    /// </summary>
    internal void Restore()
    {
        GL.glUseProgram(_lastProgram);

        SetGlEnableVertexAttribArray(0, _lastVertexAttribArrayEnabled[0] != 0);
        SetGlEnableVertexAttribArray(1, _lastVertexAttribArrayEnabled[1] != 0);
        SetGlEnableVertexAttribArray(2, _lastVertexAttribArrayEnabled[2] != 0);
        SetGlEnableVertexAttribArray(3, _lastVertexAttribArrayEnabled[3] != 0);

        SetGlEnable(GL.GL_SCISSOR_TEST, _lastScissorTest);
        SetGlEnable(GL.GL_STENCIL_TEST, _lastStencilTest);
        SetGlEnable(GL.GL_DEPTH_TEST, _lastDepthTest);
        SetGlEnable(GL.GL_CULL_FACE, _lastCullFace);
        SetGlEnable(GL.GL_BLEND, _lastBlend);

        GL.glFrontFace(_lastFrontFace);

        GL.glColorMask(_lastColorMask[0], _lastColorMask[1], _lastColorMask[2], _lastColorMask[3]);

        GL.glBindBuffer(GL.GL_ARRAY_BUFFER, _lastArrayBufferBinding); //前にバッファがバインドされていたら破棄する必要がある
        GL.glBindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, _lastElementArrayBufferBinding);

        GL.glActiveTexture(GL.GL_TEXTURE1); //テクスチャユニット1を復元
        GL.glBindTexture(GL.GL_TEXTURE_2D, _lastTexture1Binding2D);

        GL.glActiveTexture(GL.GL_TEXTURE0); //テクスチャユニット0を復元
        GL.glBindTexture(GL.GL_TEXTURE_2D, _lastTexture0Binding2D);

        GL.glActiveTexture(_lastActiveTexture);

        // restore blending
        GL.glBlendFuncSeparate(_lastBlending[0], _lastBlending[1], _lastBlending[2], _lastBlending[3]);
    }

    /// <summary>
    /// OpenGLES2の機能の有効・無効をセットする
    /// </summary>
    /// <param name="index">有効・無効にする機能</param>
    /// <param name="enabled">trueなら有効にする</param>
    internal void SetGlEnable(uint index, bool enabled)
    {
        if (enabled == true) GL.glEnable(index);
        else GL.glDisable(index);
    }

    /// <summary>
    /// OpenGLES2のVertex Attribute Array機能の有効・無効をセットする
    /// </summary>
    /// <param name="index">有効・無効にする機能</param>
    /// <param name="enabled">trueなら有効にする</param>
    internal void SetGlEnableVertexAttribArray(uint index, bool enabled)
    {
        if (enabled) GL.glEnableVertexAttribArray(index);
        else GL.glDisableVertexAttribArray(index);
    }
}
