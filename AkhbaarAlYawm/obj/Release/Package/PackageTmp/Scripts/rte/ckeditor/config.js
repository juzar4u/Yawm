/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {

    config.extraPlugins = 'lineheight';
    config.toolbar = 'MyToolbar';
    config.resize_dir = 'vertical';

    config.pasteFromWordRemoveFontStyles = false;
    config.pasteFromWordRemoveStyles = false;
    config.allowedContent = true;

    //config.fontSize_defaultLabel = '12px';
    config.font_defaultLabel = 'Georgia';
    config.skin = 'moonocolor';

    config.toolbar_MyToolbar =
    [
        ['Source', 'NewPage', '-', 'Preview', 'Print', '-', 'Templates'],
        ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord', '-', 'SpellChecker', 'Scayt'],
        ['Undo', 'Redo', '-', 'Find', 'Replace'],
        ['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'SelectionField', 'Button', 'ImageButton', 'HiddenField'],
        ['Link', 'Unlink', 'Anchor'],
        ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak'],
        '/',
        ['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript', '-', 'RemoveFormat'],
        ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote', 'CreateDiv'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
         '/',
        ['Styles', 'Format', 'Font', 'FontSize', 'lineheight'],
        { name: 'colors', items: ['TextColor', 'BGColor'] },
        //{ name: 'lineutilsplgin', items: ['lineutils'] },
        ['Maximize', 'ShowBlocks'],
    ];

};
