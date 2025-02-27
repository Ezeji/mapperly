// @ts-check

/** @type {import("@docusaurus/plugin-content-docs").SidebarsConfig} */
const sidebars = {
  docs: [
    'intro',
    {
      type: 'category',
      label: 'Getting started',
      collapsed: false,
      link: {
        type: 'generated-index',
      },
      items: [{ type: 'autogenerated', dirName: 'getting-started' }],
    },
    {
      type: 'category',
      label: 'Usage and configuration',
      collapsed: false,
      link: {
        type: 'generated-index',
      },
      items: [{ type: 'autogenerated', dirName: 'configuration' }],
    },
  ],
  api: [
    {
      type: 'category',
      label: 'API',
      collapsible: false,
      link: {
        type: 'generated-index',
      },
      items: [{ type: 'autogenerated', dirName: 'api' }],
    },
  ],
  contributing: [{ type: 'autogenerated', dirName: 'contributing' }],
};

module.exports = sidebars;
